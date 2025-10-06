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
    <Container maxWidth="md" sx={{ mt: 4, mb: 4 }}>
      <Paper elevation={3} sx={{ padding: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Editar Encuesta
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {success && (
          <Alert severity="success" sx={{ mb: 2 }}>
            {success}
          </Alert>
        )}

        <Box component="form" onSubmit={handleSubmit}>
          <Grid container spacing={3}>
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
              <Box display="flex" gap={2} justifyContent="space-between">
                <Box display="flex" gap={2}>
                  <Button
                    variant="outlined"
                    onClick={() => navigate('/encuestas')}
                    disabled={saving}
                  >
                    Volver a Lista
                  </Button>
                  <Button
                    type="submit"
                    variant="contained"
                    disabled={saving}
                  >
                    {saving ? <CircularProgress size={24} /> : 'Actualizar Encuesta'}
                  </Button>
                </Box>
                
                {/* NUEVO BOTÓN PARA GESTIONAR PREGUNTAS */}
                <Button
                  variant="outlined"
                  color="secondary"
                  startIcon={<PreguntasIcon />}
                  onClick={() => navigate(`/encuestas/${id}/preguntas`)}
                >
                  Gestionar Preguntas
                </Button>
              </Box>
            </Grid>
          </Grid>
        </Box>
      </Paper>
    </Container>
  );
};

export default EditEncuesta;