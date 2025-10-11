// src/components/encuestas/EditEncuesta.js
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
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
import { QuestionAnswer as PreguntasIcon } from '@mui/icons-material';
import { encuestasService } from '../../services/api';

const EditEncuesta = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const [formData, setFormData] = useState({
    titulo: '',
    descripcion: '',
    estado: 'Activa',
    cierraEn: ''
  });

  useEffect(() => {
    loadEncuesta();
  }, [id]);

  const loadEncuesta = async () => {
    try {
      setLoading(true);
      const encuesta = await encuestasService.getEncuesta(id);
      if (encuesta) {
        setFormData({
          titulo: encuesta.titulo,
          descripcion: encuesta.descripcion,
          estado: encuesta.estado,
          cierraEn: encuesta.cierraEn
        });
      } else {
        setError('Encuesta no encontrada');
      }
    } catch (error) {
      setError('Error al cargar la encuesta');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    setError('');
    setSuccess('');

    try {
      await encuestasService.updateEncuesta(id, formData);
      setSuccess('Encuesta actualizada exitosamente');
      setTimeout(() => {
        navigate('/encuestas');
      }, 2000);
    } catch (error) {
      setError('Error al actualizar la encuesta: ' + error.message);
    } finally {
      setSaving(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="200px">
        <CircularProgress />
      </Box>
    );
  }

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
            Gestor de Encuestas
          </Typography>
          <Typography
            component="h2"
            variant="h6"
            align="center"
            color="text.secondary"
            gutterBottom
          >
            Editar Encuesta
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
                />
              </Grid>
              <Grid item xs={12}>
                <Box display="flex" gap={2} justifyContent="space-between" sx={{ mt: 2 }}>
                  <Box display="flex" gap={2}>
                    <Button
                      variant="outlined"
                      onClick={() => navigate('/encuestas')}
                      disabled={saving}
                      sx={{
                        borderRadius: 2,
                        textTransform: 'none',
                        borderColor: '#2196F3',
                        color: '#2196F3',
                        '&:hover': { backgroundColor: '#E3F2FD', borderColor: '#1976D2' }
                      }}
                    >
                      Volver a Lista
                    </Button>
                    <Button
                      type="submit"
                      variant="contained"
                      disabled={saving}
                      sx={{
                        fontWeight: 'bold',
                        borderRadius: 2,
                        textTransform: 'none',
                        backgroundColor: '#2196F3',
                        '&:hover': { backgroundColor: '#1976D2' }
                      }}
                    >
                      {saving ? <CircularProgress size={24} color="inherit" /> : 'Actualizar Encuesta'}
                    </Button>
                  </Box>

                  <Button
                    variant="contained"
                    startIcon={<PreguntasIcon />}
                    onClick={() => navigate(`/encuestas/${id}/preguntas`)}
                    sx={{
                      borderRadius: 2,
                      textTransform: 'none',
                      backgroundColor: '#1976D2',
                      '&:hover': { backgroundColor: '#0c81cfff' }
                    }}
                  >
                    Añadir preguntas
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

export default EditEncuesta;
