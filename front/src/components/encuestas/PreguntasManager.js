import React, { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Card,
  CardContent,
  Typography,
  Button,
  Chip,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControlLabel,
  Checkbox,
  MenuItem,
  CircularProgress,
  Alert,
  Grid,
  Divider,
  // Importamos Tooltip para mejor UX en iconos
  Tooltip 
} from '@mui/material';
import {
  Add as AddIcon,
  Delete as DeleteIcon,
  RadioButtonChecked as OpcionUnicaIcon,
  CheckBox as OpcionMultipleIcon,
  ShortText as TextoIcon,
  LinearScale as EscalaIcon
} from '@mui/icons-material';
import { useParams } from 'react-router-dom';
import { preguntasService } from '../../services/api';

const PreguntasManager = () => {
  const { id } = useParams();
  const [preguntas, setPreguntas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [openDialog, setOpenDialog] = useState(false);
  const [formData, setFormData] = useState({
    enunciado: '',
    tipoPregunta: 'Texto',
    obligatorio: false
  });

  useEffect(() => {
    loadPreguntas();
  }, [id]);

  const loadPreguntas = async () => {
    try {
      setLoading(true);
      const data = await preguntasService.getPreguntasByEncuesta(id);
      // Asume que las preguntas son un array y ordena por un campo si existe, o las deja como están
      setPreguntas(data);
    } catch (error) {
      setError('Error al cargar las preguntas');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    setFormData({ enunciado: '', tipoPregunta: 'Texto', obligatorio: false });
    setOpenDialog(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      setError('');
      // Agregar manejo de opciones para otros tipos de pregunta si es necesario
      await preguntasService.createPregunta(id, formData);
      setOpenDialog(false);
      await loadPreguntas();
    } catch (error) {
      // Usar error?.message para mayor robustez
      setError('Error al crear la pregunta: ' + (error.message || 'Error desconocido'));
    }
  };

  const handleDelete = async (preguntaId) => {
    if (window.confirm('¿Seguro que deseas eliminar esta pregunta? Esta acción es irreversible.')) {
      try {
        await preguntasService.deletePregunta(preguntaId);
        await loadPreguntas();
      } catch {
        setError('Error al eliminar la pregunta');
      }
    }
  };

  const getTipoIcon = (tipo) => {
    switch (tipo) {
      case 'SeleccionUnica': return <OpcionUnicaIcon />;
      case 'OpcionMultiple': return <OpcionMultipleIcon />;
      case 'Texto': return <TextoIcon />;
      case 'Escala': return <EscalaIcon />;
      default: return <TextoIcon />;
    }
  };
  
  const getTipoLabel = (tipo) => {
    switch (tipo) {
        case 'SeleccionUnica': return 'Selección Única';
        case 'OpcionMultiple': return 'Opción Múltiple';
        case 'Texto': return 'Texto Libre';
        case 'Escala': return 'Escala Numérica';
        default: return 'Texto';
    }
  };

  const getTipoColor = (tipo) => {
    switch (tipo) {
      case 'SeleccionUnica': return 'primary';
      case 'OpcionMultiple': return 'secondary';
      case 'Texto': return 'info'; // Cambio de success a info para variar
      case 'Escala': return 'warning';
      default: return 'default';
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="300px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ p: { xs: 2, md: 3, lg: 4 } }}>
      
      {/* Nuevo contenedor para centrar el título y alinear el botón */}
      <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', mb: 4 }}>
        <Typography 
          variant="h4" 
          fontWeight="bold" 
          textAlign="center" 
          color="text.primary"
          sx={{ mb: 2 }}
        >
          Preguntas de la Encuesta
        </Typography>

        <Button
          variant="contained"
          startIcon={<AddIcon />}
          sx={{ 
            bgcolor: '#6fcf97', 
            '&:hover': { bgcolor: '#57b87f' },
            boxShadow: 3,
            fontWeight: 'bold'
          }}
          onClick={handleCreate}
        >
          Agregar Pregunta
        </Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 3, borderRadius: 2 }}>{error}</Alert>}

      {preguntas.length === 0 ? (
        <Paper sx={{ p: 6, textAlign: 'center', borderRadius: 3, boxShadow: 3 }}>
          <Typography variant="h5" color="text.secondary" gutterBottom>
            Aún no hay preguntas.
          </Typography>
          <Typography variant="body1" color="text.secondary" mb={3}>
            ¡Comienza a construir tu encuesta añadiendo la primera pregunta!
          </Typography>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            sx={{ bgcolor: '#6fcf97', '&:hover': { bgcolor: '#57b87f' }, fontWeight: 'bold' }}
            onClick={handleCreate}
          >
            Agregar Primera Pregunta
          </Button>
        </Paper>
      ) : (
        <Grid container spacing={4}>
          {preguntas.map((pregunta) => (
            <Grid item xs={12} md={6} lg={4} key={pregunta.id}>
              <Card
                sx={{
                  p: 2,
                  borderRadius: 3,
                  boxShadow: 4, // Sutilmente más de lo anterior
                  transition: '0.3s',
                  borderLeft: `5px solid ${getTipoColor(pregunta.tipoPregunta)}`, // Indicador de tipo
                  '&:hover': { boxShadow: 8, transform: 'translateY(-2px)' }
                }}
              >
                <CardContent sx={{ p: 1 }}>
                  {/* Encabezado y Opciones */}
                  <Box display="flex" flexDirection="column" gap={1}>
                    <Box display="flex" alignItems="center" justifyContent="space-between">
                      <Box display="flex" alignItems="center" gap={1.5}>
                         <Tooltip title={getTipoLabel(pregunta.tipoPregunta)}>
                            <Box color={`${getTipoColor(pregunta.tipoPregunta)}.main`}>
                                {getTipoIcon(pregunta.tipoPregunta)}
                            </Box>
                         </Tooltip>
                        <Typography 
                            variant="subtitle1" 
                            fontWeight="bold" 
                            sx={{ wordBreak: 'break-word' }}
                        >
                            {pregunta.enunciado}
                        </Typography>
                      </Box>
                      {pregunta.obligatorio && (
                         <Tooltip title="Esta pregunta es obligatoria">
                            <Chip label="Obligatoria" color="error" size="small" variant="outlined" />
                         </Tooltip>
                      )}
                    </Box>

                    <Box mt={1} display="flex" alignItems="center" flexWrap="wrap" gap={1}>
                       <Chip
                          label={getTipoLabel(pregunta.tipoPregunta)}
                          color={getTipoColor(pregunta.tipoPregunta)}
                          size="small"
                          variant="filled"
                        />
                        {pregunta.opciones && pregunta.opciones.length > 0 && (
                          <Chip 
                            label={`Opciones: ${pregunta.opciones.length}`} 
                            size="small" 
                            variant="outlined"
                          />
                        )}
                    </Box>

                    {pregunta.opciones && pregunta.opciones.length > 0 && (
                       <Typography variant="caption" display="block" mt={1} color="text.secondary" sx={{ 
                           overflow: 'hidden', 
                           textOverflow: 'ellipsis', 
                           whiteSpace: 'nowrap' 
                       }}>
                           Ejemplos: {pregunta.opciones.map(o => o.label).join(', ')}
                       </Typography>
                    )}
                  </Box>

                  <Divider sx={{ my: 2 }} />

                  {/* Acciones */}
                  <Box display="flex" justifyContent="flex-end">
                    <Tooltip title="Eliminar Pregunta">
                        <IconButton color="error" onClick={() => handleDelete(pregunta.id)}>
                          <DeleteIcon />
                        </IconButton>
                    </Tooltip>
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      {/* Diálogo de Creación */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle sx={{ fontWeight: 'bold' }}>Crear Nueva Pregunta</DialogTitle>
        <form onSubmit={handleSubmit}>
          <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 3, pt: 2 }}>
            <TextField
              label="Enunciado de la Pregunta"
              fullWidth
              multiline
              rows={2}
              required
              value={formData.enunciado}
              onChange={(e) => setFormData({ ...formData, enunciado: e.target.value })}
              variant="outlined"
            />
            <TextField
              select
              label="Tipo de Pregunta"
              fullWidth
              required
              value={formData.tipoPregunta}
              onChange={(e) => setFormData({ ...formData, tipoPregunta: e.target.value })}
              variant="outlined"
            >
              <MenuItem value="Texto">Texto Libre</MenuItem>
              <MenuItem value="SeleccionUnica">Selección Única</MenuItem>
              <MenuItem value="OpcionMultiple">Opción Múltiple</MenuItem>
              <MenuItem value="Escala">Escala Numérica</MenuItem>
            </TextField>
            <FormControlLabel
              control={
                <Checkbox
                  checked={formData.obligatorio}
                  onChange={(e) => setFormData({ ...formData, obligatorio: e.target.checked })}
                  color="error" // Usar un color que resalte la obligatoriedad
                />
              }
              label="Marcar como Pregunta Obligatoria"
            />
          </DialogContent>
          <DialogActions sx={{ p: 3, borderTop: '1px solid #eee' }}>
            <Button onClick={() => setOpenDialog(false)} color="inherit">Cancelar</Button>
            <Button 
                type="submit" 
                variant="contained" 
                startIcon={<AddIcon />}
                sx={{ bgcolor: '#6fcf97', '&:hover': { bgcolor: '#57b87f' }, fontWeight: 'bold' }}
            >
              Crear Pregunta
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default PreguntasManager;