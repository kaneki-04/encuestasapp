import React, { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Chip,
  Grid,
  IconButton,
  CircularProgress,
  Alert,
  Container
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  BarChart as ChartIcon,
  Logout as LogoutIcon
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { encuestasService } from '../../services/api';
import { useAuth } from '../../contexts/AuthContext';

const EncuestasList = () => {
  const [encuestas, setEncuestas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const { logout } = useAuth();

  useEffect(() => {
    loadEncuestas();
  }, []);

  const loadEncuestas = async () => {
    try {
      setLoading(true);
      const data = await encuestasService.getEncuestas();
      setEncuestas(data);
    } catch (error) {
      setError('Error al cargar las encuestas');
      console.error('Error:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    navigate('/encuestas/create');
  };

  const handleEdit = (id) => {
    navigate(`/encuestas/edit/${id}`);
  };

  const handleDelete = async (id) => {
    if (window.confirm('¿Estás seguro de que quieres eliminar esta encuesta?')) {
      try {
        await encuestasService.deleteEncuesta(id);
        await loadEncuestas();
      } catch (error) {
        setError('Error al eliminar la encuesta');
      }
    }
  };

  const handleLogout = async () => {
    try {
      await logout();
      navigate('/login');
    } catch (error) {
      console.error('Error en logout:', error);
    }
  };

  const getEstadoColor = (estado) => {
    switch (estado) {
      case 'Activa': return 'success';
      case 'Inactiva': return 'default';
      case 'Finalizada': return 'secondary';
      default: return 'default';
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="200px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
        <Typography variant="h4" component="h1">
          Mis Encuestas
        </Typography>
        <Box display="flex" gap={2}>
          <Button
            variant="outlined"
            color="secondary"
            startIcon={<LogoutIcon />}
            onClick={handleLogout}
          >
            Cerrar Sesión
          </Button>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={handleCreate}
          >
            Nueva Encuesta
          </Button>
        </Box>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Grid container spacing={3}>
        {encuestas.length === 0 ? (
          <Grid item xs={12}>
            <Card>
              <CardContent sx={{ textAlign: 'center', py: 4 }}>
                <Typography variant="h6" color="textSecondary" gutterBottom>
                  No tienes encuestas creadas
                </Typography>
                <Typography variant="body2" color="textSecondary" sx={{ mb: 2 }}>
                  Comienza creando tu primera encuesta
                </Typography>
                <Button
                  variant="contained"
                  startIcon={<AddIcon />}
                  onClick={handleCreate}
                >
                  Crear Primera Encuesta
                </Button>
              </CardContent>
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
                  '&:hover': {
                    boxShadow: 6,
                    transform: 'translateY(-2px)'
                  }
                }}
              >
                <CardContent sx={{ flexGrow: 1 }}>
                  <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={2}>
                    <Typography variant="h6" component="h2" sx={{ flex: 1 }}>
                      {encuesta.titulo}
                    </Typography>
                    <Chip 
                      label={encuesta.estado}
                      color={getEstadoColor(encuesta.estado)}
                      size="small"
                    />
                  </Box>

                  <Typography 
                    variant="body2" 
                    color="textSecondary" 
                    sx={{ mb: 2, minHeight: '40px' }}
                  >
                    {encuesta.descripcion || 'Sin descripción'}
                  </Typography>

                  <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
                    <Typography variant="caption" color="textSecondary">
                      Creada: {new Date(encuesta.creadoEn).toLocaleDateString()}
                    </Typography>
                    <Typography variant="caption" color="textSecondary">
                      Respuestas: {encuesta.totalRespuestas || 0}
                    </Typography>
                  </Box>

                  <Box display="flex" justifyContent="space-between" alignItems="center">
                    <Typography variant="caption" color="textSecondary">
                      Cierra: {new Date(encuesta.cierraEn).toLocaleDateString()}
                    </Typography>
                  </Box>
                </CardContent>

                <Box sx={{ p: 1, bgcolor: 'grey.50' }}>
                  <Box display="flex" justifyContent="space-between">
                    <IconButton 
                      size="small" 
                      color="primary"
                      onClick={() => handleEdit(encuesta.id)}
                    >
                      <EditIcon />
                    </IconButton>
                    
                    <IconButton size="small" color="info">
                      <ChartIcon />
                    </IconButton>
                    
                    <IconButton 
                      size="small" 
                      color="error"
                      onClick={() => handleDelete(encuesta.id)}
                    >
                      <DeleteIcon />
                    </IconButton>
                  </Box>
                </Box>
              </Card>
            </Grid>
          ))
        )}
      </Grid>
    </Container>
  );
};

export default EncuestasList;