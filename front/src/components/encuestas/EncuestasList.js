import React, { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Card,
  CardContent,
  Typography,
  Button,
  Chip,
  Grid,
  IconButton,
  CircularProgress,
  Alert,
  Container,
  Menu,
  MenuItem,
  Divider,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  BarChart as ChartIcon,
  Logout as LogoutIcon,
  MoreVert as MoreIcon,
  QuestionAnswer as PreguntasIcon,
  PlayArrow as ResponderIcon,
  List as RespuestasIcon,
  FileDownload as FileDownloadIcon,
  AccessTime as AccessTimeIcon,
  AccountTree as StructureIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { encuestasService } from '../../services/api';
import { useAuth } from '../../contexts/AuthContext';
import ExportEncuestaModal from './ExportEncuestaModal';

const EncuestasList = () => {
  const [encuestas, setEncuestas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [menuAnchor, setMenuAnchor] = useState(null);
  const [encuestaSeleccionada, setEncuestaSeleccionada] = useState(null);
  const [openExportModal, setOpenExportModal] = useState(false);
  const navigate = useNavigate();
  const { logout } = useAuth();

  const PRIMARY_GREEN = '#2e7d32'; 
  const HOVER_GREEN = '#4c8c4c';
  const LIGHT_GREEN = '#e8f5e9';

  useEffect(() => {
    loadEncuestas();
  }, []);

  const loadEncuestas = async () => {
    try {
      setLoading(true);
      
      // *** CORRECCIÓN: Se elimina el mockData y se usa la llamada real a la API. ***
      const data = await encuestasService.getEncuestas();
      
      setEncuestas(data);
    } catch (error) {
      setError('Error al cargar las encuestas');
      console.error('Error:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => navigate('/encuestas/create');
  const handleEdit = (id) => navigate(`/encuestas/edit/${id}`);
  const handleDelete = async (id) => {
    if (window.confirm('¿Estás seguro de que quieres eliminar esta encuesta?')) {
      try {
        await encuestasService.deleteEncuesta(id); // Llamada real a delete
        await loadEncuestas();
      } catch {
        setError('Error al eliminar la encuesta...');
      }
    }
  };
  const handleMenuOpen = (event, encuesta) => {
    setMenuAnchor(event.currentTarget);
    setEncuestaSeleccionada(encuesta);
  };
  const handleMenuClose = () => {
    setMenuAnchor(null);
    setEncuestaSeleccionada(null);
  };
  const handleGestionPreguntas = () => {
    handleMenuClose();
    encuestaSeleccionada && navigate(`/encuestas/${encuestaSeleccionada.id}/preguntas`);
  }
  const handleVerEstadisticas = () => {
    handleMenuClose();
    encuestaSeleccionada && navigate(`/encuestas/${encuestaSeleccionada.id}/estadisticas`);
  }
  const handleResponderEncuesta = () => {
    handleMenuClose();
    encuestaSeleccionada && navigate(`/encuestas/${encuestaSeleccionada.id}/responder`);
  }
  const handleMisRespuestas = () => navigate('/mis-respuestas');
  const handleLogout = async () => { await logout(); navigate('/login'); };


  const getEstadoColor = (estado) => {
    switch (estado) {
      case 'Activa': return 'success';
      case 'Inactiva': return 'warning';
      case 'Finalizada': return 'error';
      default: return 'default';
    }
  };

  if (loading) return (
    <Box display="flex" justifyContent="center" alignItems="center" minHeight="50vh">
      <CircularProgress sx={{ color: PRIMARY_GREEN }} />
    </Box>
  );

  return (
    <Container component="main" maxWidth="lg" sx={{ mt: 5, mb: 4 }}>
      <Paper elevation={1} sx={{ p: { xs: 2, sm: 4 }, borderRadius: 3, border: `1px solid ${LIGHT_GREEN}` }}>
        {/* Header */}
        <Box 
          display="flex" 
          justifyContent="space-between" 
          alignItems="center" 
          flexWrap="wrap" 
          gap={2}
          mb={4}
        >
          {/* Título y Subtítulo mejorados */}
          <Box>
            <Typography variant="h4" sx={{ fontWeight: 'bold', color: PRIMARY_GREEN }}>
              EncuestApp
            </Typography>
            <Typography variant="subtitle1" color="text.secondary">
               Gestión todas tus encuestas
            </Typography>
          </Box>
          
          {/* Grupo de Botones de Acción */}
          <Box display="flex" gap={1.5} flexWrap="wrap" justifyContent="flex-end">
            
            {/* 1. Botón Principal (Crear) - Contained */}
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={handleCreate}
              sx={{
                py: 1.2,
                backgroundColor: PRIMARY_GREEN,
                color: '#ffffffff',
                fontWeight: 'bold',
                '&:hover': {
                  backgroundColor: HOVER_GREEN
                }
              }}
            >
              Crear Encuesta
            </Button>
            
            {/* 2. Botones Secundarios (Outlined/Text) */}
            <Button
              variant="outlined"
              startIcon={<RespuestasIcon />}
              onClick={handleMisRespuestas}
              sx={{
                 color: PRIMARY_GREEN,
                 borderColor: PRIMARY_GREEN,
                 '&:hover': {
                    backgroundColor: LIGHT_GREEN,
                    borderColor: HOVER_GREEN,
                 }
              }}
            >
              Respuestas de las encuestas
            </Button>

            <Button
              variant="outlined"
              startIcon={<FileDownloadIcon />}
              onClick={() => setOpenExportModal(true)}
              sx={{
                 color: PRIMARY_GREEN,
                 borderColor: PRIMARY_GREEN,
                 '&:hover': {
                    backgroundColor: LIGHT_GREEN,
                    borderColor: HOVER_GREEN,
                 }
              }}
            >
              Exportar
            </Button>

            {/* 3. Botón de Logout - Menos prominente */}
            <IconButton 
              color="error" 
              onClick={handleLogout} 
              title="Cerrar Sesión"
              sx={{ p: 1, border: '1px solid #f4433650', borderRadius: 2 }}
            >
                <LogoutIcon />
            </IconButton>
          </Box>
        </Box>
        <Divider sx={{ mb: 4 }} /> {/* Separador visual */}

        {error && (
          <Alert severity="error" sx={{ mb: 2, borderRadius: 2 }}>{error}</Alert>
        )}

        {/* Listado de Encuestas */}
        <Grid container spacing={3}>
          {encuestas.length === 0 ? (
            <Grid item xs={12}>
              <Card sx={{ p: 5, textAlign: 'center', borderRadius: 3, border: `2px dashed ${LIGHT_GREEN}` }}>
                <Typography variant="h5" color="text.secondary" gutterBottom>
                  Crea tu Primera Encuesta!
                </Typography>
                <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
                  No tienes encuestas activas, Usa el botón de arriba para comenzar.
                </Typography>
                <Button
                  variant="contained"
                  startIcon={<AddIcon />}
                  onClick={handleCreate}
                  sx={{
                    backgroundColor: PRIMARY_GREEN,
                    color: '#fff',
                    '&:hover': { backgroundColor: HOVER_GREEN }
                  }}
                >
                  Comenzar
                </Button>
              </Card>
            </Grid>
          ) : (
            encuestas.map((encuesta) => (
              <Grid item xs={12} md={6} lg={4} key={encuesta.id}>
                <Card
                  sx={{
                    height: '100%',
                    display: 'flex',
                    flexDirection: 'column',
                    transition: '0.3s',
                    borderRadius: 3,
                    borderLeft: `5px solid ${getEstadoColor(encuesta.estado) === 'success' ? PRIMARY_GREEN : '#ccc'}`,
                    '&:hover': { boxShadow: 8, transform: 'translateY(-4px)' }
                  }}
                >
                  <CardContent sx={{ flexGrow: 1, pb: 1 }}>
                    {/* Título y Estado */}
                    <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={1}>
                      <Typography variant="h6" component="h2" sx={{ 
                          flex: 1, 
                          fontWeight: 'bold', 
                          color: PRIMARY_GREEN,
                          lineHeight: 1.3
                      }}>
                        {encuesta.titulo}
                      </Typography>
                      <Chip 
                          label={encuesta.estado} 
                          color={getEstadoColor(encuesta.estado)} 
                          size="small" 
                          sx={{ fontWeight: 'bold' }}
                      />
                    </Box>

                    <Typography variant="body2" color="text.secondary" sx={{ minHeight: '40px', mb: 2 }}>
                      {encuesta.descripcion || 'Sin descripción.'}
                    </Typography>

                    {/* Metadatos (Respuestas, Preguntas, Fecha) */}
                    <Grid container spacing={1} sx={{ mt: 2 }}>
                        <Grid item xs={6}>
                            <Box display="flex" alignItems="center" gap={0.5}>
                                <RespuestasIcon fontSize="small" color="action" />
                                <Typography variant="caption" color="text.secondary" fontWeight="bold">
                                    {encuesta.totalRespuestas || 0} Respuestas
                                </Typography>
                            </Box>
                        </Grid>
                        <Grid item xs={6}>
                            <Box display="flex" alignItems="center" gap={0.5}>
                                <StructureIcon fontSize="small" color="action" />
                                <Typography variant="caption" color="text.secondary" fontWeight="bold">
                                    {encuesta.totalPreguntas || 0} Preguntas
                                </Typography>
                            </Box>
                        </Grid>
                        <Grid item xs={12}>
                            <Box display="flex" alignItems="center" gap={0.5} mt={1}>
                                <AccessTimeIcon fontSize="small" color="action" />
                                <Typography variant="caption" color="text.secondary">
                                    Cierra: {encuesta.cierraEn ? new Date(encuesta.cierraEn).toLocaleDateString() : 'N/A'}
                                </Typography>
                            </Box>
                        </Grid>
                    </Grid>
                  </CardContent>

                  {/* Pie de la Tarjeta con Acciones */}
                  <Box sx={{ p: 1, bgcolor: LIGHT_GREEN, borderRadius: '0 0 10px 10px' }}>
                    <Box display="flex" justifyContent="space-between" alignItems="center">
                      
                      {/* Acciones Rápidas (Responder y Estadísticas) */}
                      <Box>
                        <IconButton 
                          size="medium" 
                          sx={{ color: PRIMARY_GREEN }}
                          onClick={() => navigate(`/encuestas/${encuesta.id}/responder`)} 
                          title="Responder Encuesta"
                        >
                          <ResponderIcon />
                        </IconButton>
                        <IconButton 
                          size="medium" 
                          color="info" 
                          onClick={() => navigate(`/encuestas/${encuesta.id}/estadisticas`)} 
                          title="Ver Estadísticas"
                        >
                          <ChartIcon />
                        </IconButton>
                      </Box>
                      
                      {/* Acciones de Gestión (Editar, Eliminar, Más) */}
                      <Box>
                        <IconButton size="medium" sx={{ color: PRIMARY_GREEN }} onClick={() => handleEdit(encuesta.id)} title="Editar Encuesta">
                          <EditIcon />
                        </IconButton>
                        <IconButton size="medium" color="error" onClick={() => handleDelete(encuesta.id)} title="Eliminar Encuesta">
                          <DeleteIcon />
                        </IconButton>
                        <IconButton 
                            size="medium" 
                            onClick={(e) => handleMenuOpen(e, encuesta)} 
                            title="Más opciones"
                            sx={{ color: 'grey.700' }}
                        >
                          <MoreIcon />
                        </IconButton>
                      </Box>
                    </Box>
                  </Box>
                </Card>
              </Grid>
            ))
          )}
        </Grid>

        {/* Menú de opciones adicionales */}
        <Menu 
            anchorEl={menuAnchor} 
            open={Boolean(menuAnchor)} 
            onClose={handleMenuClose}
            anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
            transformOrigin={{ vertical: 'top', horizontal: 'right' }}
        >
          <MenuItem onClick={handleGestionPreguntas}>
              <PreguntasIcon sx={{ mr: 1, color: PRIMARY_GREEN }} /> 
              Gestionar Preguntas
          </MenuItem>
          <MenuItem onClick={handleVerEstadisticas}>
              <ChartIcon sx={{ mr: 1, color: 'info.main' }} /> 
              Ver Estadísticas
          </MenuItem>
          <MenuItem onClick={handleResponderEncuesta}>
              <ResponderIcon sx={{ mr: 1, color: 'primary.main' }} /> 
              Responder Encuesta
          </MenuItem>
          <Divider />
          <MenuItem onClick={() => encuestaSeleccionada && handleDelete(encuestaSeleccionada.id)} sx={{ color: 'error.main' }}>
              <DeleteIcon sx={{ mr: 1 }} /> 
              Eliminar (Menú)
          </MenuItem>
        </Menu>
      </Paper>

      {/* Modal de exportación */}
      <ExportEncuestaModal open={openExportModal} onClose={() => setOpenExportModal(false)} />
    </Container>
  );
};

export default EncuestasList;