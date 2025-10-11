// src/components/encuestas/CreateEncuesta.js
import React, { useState } from 'react';
import {
  Box,
  Paper,
  TextField,
  Button,
  Typography,
  Container,
  Alert,
  CircularProgress,
  MenuItem,
  Grid
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { encuestasService } from '../../services/api';

const CreateEncuesta = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    titulo: '',
    descripcion: '',
    estado: 'Activa',
    cierraEn: ''
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setSuccess('');

    try {
      const cierraEn = new Date();
      cierraEn.setDate(cierraEn.getDate() + 7);

      const encuestaData = {
        ...formData,
        cierraEn: formData.cierraEn || cierraEn.toISOString().split('T')[0],
      };

      const result = await encuestasService.createEncuesta(encuestaData);

      if (result.success) {
        setSuccess('Encuesta creada exitosamente. Redirigiendo...');
        setTimeout(() => {
          if (result.id) {
            navigate(`/encuestas/${result.id}/preguntas`);
          } else {
            navigate('/encuestas');
          }
        }, 2000);
      } else {
        setError('Error al crear la encuesta: ' + (result.message || 'Error desconocido'));
      }
    } catch (error) {
      setError('Error al crear la encuesta: ' + error.message);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  return (
    <Container component="main" maxWidth="sm">
      <Box
        sx={{
          marginTop: 10,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          minHeight: '80vh',
          justifyContent: 'center',
        }}
      >
        <Paper
          elevation={10}
          sx={{
            padding: 4,
            width: '100%',
            borderRadius: 3,
          }}
        >
          <Typography
            component="h1"
            variant="h5"
            align="center"
            sx={{ fontWeight: 'bold', mb: 1 }}
          >
            Gestor de EncuestApps
          </Typography>

          <Typography
            component="h2"
            variant="h6"
            align="center"
            color="text.secondary"
            sx={{ mb: 3 }}
          >
            Crea tu nueva encuesta
          </Typography>

          {error && (
            <Alert severity="error" sx={{ mb: 2, borderRadius: 2 }}>
              {error}
            </Alert>
          )}

          {success && (
            <Alert severity="success" sx={{ mb: 2, borderRadius: 2 }}>
              {success}
            </Alert>
          )}

          <Box component="form" onSubmit={handleSubmit}>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <TextField
                  required
                  fullWidth
                  id="titulo"
                  label="Título de la Encuesta"
                  name="titulo"
                  value={formData.titulo}
                  onChange={handleChange}
                  helperText="Ingresa un título descriptivo"
                />
              </Grid>

              <Grid item xs={12}>
                <TextField
                  fullWidth
                  multiline
                  rows={4}
                  id="descripcion"
                  label="Descripción"
                  name="descripcion"
                  value={formData.descripcion}
                  onChange={handleChange}
                  helperText="Describe brevemente el propósito de la encuesta"
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  required
                  fullWidth
                  select
                  id="estado"
                  label="Estado"
                  name="estado"
                  value={formData.estado}
                  onChange={handleChange}
                >
                  <MenuItem value="Activa">Activa</MenuItem>
                  <MenuItem value="Inactiva">Inactiva</MenuItem>
                  <MenuItem value="Finalizada">Finalizada</MenuItem>
                </TextField>
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  required
                  fullWidth
                  id="cierraEn"
                  label="Fecha de Cierre"
                  name="cierraEn"
                  type="date"
                  InputLabelProps={{ shrink: true }}
                  value={formData.cierraEn}
                  onChange={handleChange}
                  helperText="Fecha en que finalizará la encuesta"
                />
              </Grid>

              {/* Botones ordenados y alineados */}
              <Grid item xs={12}>
                <Box display="flex" justifyContent="flex-end" gap={2} sx={{ mt: 3 }}>
                  <Button
                    variant="outlined"
                    color="inherit"
                    onClick={() => navigate('/encuestas')}
                    disabled={loading}
                    sx={{
                      textTransform: 'none',
                      borderRadius: 2,
                      px: 3,
                      py: 1,
                    }}
                  >
                    Cancelar
                  </Button>

                  <Button
                    type="submit"
                    variant="contained"
                    color="primary"
                    disabled={loading}
                    sx={{
                      textTransform: 'none',
                      borderRadius: 2,
                      px: 3,
                      py: 1,
                      fontWeight: 'bold',
                      backgroundColor: '#1976d2',
                      '&:hover': { backgroundColor: '#125ea2' },
                    }}
                  >
                    {loading ? (
                      <CircularProgress size={24} color="inherit" />
                    ) : (
                      'Siguiente'
                    )}
                  </Button>
                </Box>
              </Grid>
            </Grid>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
};

export default CreateEncuesta;
