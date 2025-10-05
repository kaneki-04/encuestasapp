import React, { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  List,
  ListItem,
  ListItemText,
  Chip,
  Container,
  CircularProgress,
  Alert,
  Divider,
  Card,
  CardContent
} from '@mui/material';
import {
  Assignment as EncuestaIcon,
  QuestionAnswer as RespuestaIcon
} from '@mui/icons-material';
import { respuestasService } from '../../services/api';

const MisRespuestas = () => {
  const [respuestas, setRespuestas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadRespuestas();
  }, []);

  const loadRespuestas = async () => {
    try {
      setLoading(true);
      const data = await respuestasService.getMisRespuestas();
      setRespuestas(data);
    } catch (error) {
      setError('Error al cargar las respuestas');
    } finally {
      setLoading(false);
    }
  };

  const formatFecha = (fechaString) => {
    return new Date(fechaString).toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Mis Respuestas
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}

      {respuestas.length === 0 ? (
        <Card>
          <CardContent sx={{ textAlign: 'center', py: 4 }}>
            <RespuestaIcon sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
            <Typography variant="h6" color="textSecondary" gutterBottom>
              No has respondido encuestas aún
            </Typography>
            <Typography variant="body2" color="textSecondary">
              Cuando respondas encuestas, podrás ver tu historial aquí.
            </Typography>
          </CardContent>
        </Card>
      ) : (
        <Box>
          <Typography variant="body1" color="textSecondary" sx={{ mb: 3 }}>
            Has respondido {respuestas.length} encuesta{respuestas.length !== 1 ? 's' : ''}
          </Typography>

          {respuestas.map((respuesta, index) => (
            <Paper key={respuesta.id} sx={{ mb: 3, p: 3 }} elevation={2}>
              <Box display="flex" alignItems="flex-start" gap={2}>
                <EncuestaIcon color="primary" sx={{ mt: 0.5 }} />
                <Box flex={1}>
                  <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={1}>
                    <Typography variant="h6" component="h2">
                      {respuesta.encuesta.titulo}
                    </Typography>
                    <Chip 
                      label={formatFecha(respuesta.fechaRespuesta)}
                      variant="outlined"
                      size="small"
                    />
                  </Box>
                  
                  <Typography variant="body2" color="textSecondary" paragraph>
                    Pregunta: {respuesta.pregunta.enunciado}
                  </Typography>

                  <Paper variant="outlined" sx={{ p: 2, bgcolor: 'grey.50' }}>
                    <Typography variant="subtitle2" color="textSecondary" gutterBottom>
                      Tu respuesta:
                    </Typography>
                    <Typography variant="body1">
                      {respuesta.texto || 'Sin respuesta'}
                    </Typography>
                  </Paper>
                </Box>
              </Box>

              {index < respuestas.length - 1 && (
                <Divider sx={{ mt: 2 }} />
              )}
            </Paper>
          ))}
        </Box>
      )}
    </Container>
  );
};

export default MisRespuestas;