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
      // Calcular fecha de cierre (7 días desde hoy)
      const cierraEn = new Date();
      cierraEn.setDate(cierraEn.getDate() + 7);
      
      const encuestaData = {
        ...formData,
        cierraEn: formData.cierraEn || cierraEn.toISOString().split('T')[0]
      };

      const result = await encuestasService.createEncuesta(encuestaData);
      
      if (result.success) {
        setSuccess('Encuesta creada exitosamente. Redirigiendo...');
        
        // Redirigir después de 2 segundos
        setTimeout(() => {
          if (result.id) {
            // Si tenemos el ID, redirigir a gestionar preguntas
            navigate(`/encuestas/${result.id}/preguntas`);
          } else {
            // Si no tenemos el ID, redirigir a la lista
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
      [name]: value
    }));
  };

  return (
    <Container maxWidth="md" sx={{ mt: 4, mb: 4 }}>
      <Paper elevation={3} sx={{ padding: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Crear Nueva Encuesta
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
                helperText="Ingresa un título descriptivo para tu encuesta"
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
                helperText="Describe el propósito de esta encuesta"
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
                helperText="Fecha cuando la encuesta dejará de estar activa"
              />
            </Grid>
            <Grid item xs={12}>
              <Box display="flex" gap={2} justifyContent="flex-end">
                <Button
                  variant="outlined"
                  onClick={() => navigate('/encuestas')}
                  disabled={loading}
                >
                  Cancelar
                </Button>
                <Button
                  type="submit"
                  variant="contained"
                  disabled={loading}
                >
                  {loading ? <CircularProgress size={24} /> : 'Crear Encuesta'}
                </Button>
              </Box>
            </Grid>
          </Grid>
        </Box>
      </Paper>
    </Container>
  );
};

export default CreateEncuesta;