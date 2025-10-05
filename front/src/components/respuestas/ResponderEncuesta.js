import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  Button,
  TextField,
  Radio,
  RadioGroup,
  FormControlLabel,
  FormControl,
  FormLabel,
  Checkbox,
  Alert,
  CircularProgress,
  Container,
  Slider,
  Chip
} from '@mui/material';
import { respuestasService, encuestasService } from '../../services/api';

const ResponderEncuesta = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [encuesta, setEncuesta] = useState(null);
  const [preguntas, setPreguntas] = useState([]);
  const [respuestas, setRespuestas] = useState({});
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  useEffect(() => {
    loadEncuesta();
  }, [id]);

  const loadEncuesta = async () => {
    try {
      setLoading(true);
      // Cargar encuesta
      const encuestaData = await encuestasService.getEncuesta(id);
      setEncuesta(encuestaData);
      
      // Cargar preguntas (usando el servicio de preguntas que crearemos)
      const preguntasData = await getPreguntasEjemplo(id); // Temporal
      setPreguntas(preguntasData);
      
      // Inicializar respuestas vacías
      const respuestasIniciales = {};
      preguntasData.forEach(pregunta => {
        if (pregunta.tipoPregunta === 'OpcionMultiple') {
          respuestasIniciales[pregunta.id] = [];
        } else {
          respuestasIniciales[pregunta.id] = '';
        }
      });
      setRespuestas(respuestasIniciales);
    } catch (error) {
      setError('Error al cargar la encuesta');
    } finally {
      setLoading(false);
    }
  };

  const handleRespuestaChange = (preguntaId, value) => {
    setRespuestas(prev => ({
      ...prev,
      [preguntaId]: value
    }));
  };

  const handleOpcionMultipleChange = (preguntaId, opcionValue, checked) => {
    setRespuestas(prev => {
      const currentValues = prev[preguntaId] || [];
      let newValues;
      
      if (checked) {
        newValues = [...currentValues, opcionValue];
      } else {
        newValues = currentValues.filter(v => v !== opcionValue);
      }
      
      return {
        ...prev,
        [preguntaId]: newValues
      };
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    // Validar preguntas obligatorias
    const preguntasObligatorias = preguntas.filter(p => p.obligatorio);
    const errores = preguntasObligatorias.filter(p => {
      const respuesta = respuestas[p.id];
      return !respuesta || (Array.isArray(respuesta) && respuesta.length === 0);
    });

    if (errores.length > 0) {
      setError(`Debes responder las preguntas obligatorias: ${errores.map(p => p.enunciado).join(', ')}`);
      return;
    }

    try {
      setSubmitting(true);
      setError('');
      
      const respuestasFormateadas = preguntas.map(pregunta => ({
        preguntaId: pregunta.id,
        respuestaTexto: Array.isArray(respuestas[pregunta.id]) 
          ? respuestas[pregunta.id].join(', ')
          : respuestas[pregunta.id],
        respuestaOpcionId: !Array.isArray(respuestas[pregunta.id]) && respuestas[pregunta.id] ? respuestas[pregunta.id] : ''
      }));

      await respuestasService.responderEncuesta(id, respuestasFormateadas);
      
      setSuccess('¡Encuesta respondida exitosamente!');
      setTimeout(() => {
        navigate('/encuestas');
      }, 2000);
    } catch (error) {
      setError('Error al enviar las respuestas: ' + error.message);
    } finally {
      setSubmitting(false);
    }
  };

  const renderPregunta = (pregunta) => {
    switch (pregunta.tipoPregunta) {
      case 'Texto':
        return (
          <TextField
            fullWidth
            multiline
            rows={3}
            placeholder="Escribe tu respuesta aquí..."
            value={respuestas[pregunta.id] || ''}
            onChange={(e) => handleRespuestaChange(pregunta.id, e.target.value)}
            variant="outlined"
          />
        );

      case 'SeleccionUnica':
        return (
          <FormControl component="fieldset">
            <RadioGroup
              value={respuestas[pregunta.id] || ''}
              onChange={(e) => handleRespuestaChange(pregunta.id, e.target.value)}
            >
              {pregunta.opciones.map((opcion) => (
                <FormControlLabel
                  key={opcion.id}
                  value={opcion.value}
                  control={<Radio />}
                  label={opcion.label}
                />
              ))}
            </RadioGroup>
          </FormControl>
        );

      case 'OpcionMultiple':
        return (
          <Box>
            {pregunta.opciones.map((opcion) => (
              <FormControlLabel
                key={opcion.id}
                control={
                  <Checkbox
                    checked={(respuestas[pregunta.id] || []).includes(opcion.value)}
                    onChange={(e) => handleOpcionMultipleChange(
                      pregunta.id, 
                      opcion.value, 
                      e.target.checked
                    )}
                  />
                }
                label={opcion.label}
              />
            ))}
          </Box>
        );

      case 'Escala':
        return (
          <Box sx={{ px: 2 }}>
            <Slider
              value={respuestas[pregunta.id] ? parseInt(respuestas[pregunta.id]) : 3}
              onChange={(_, value) => handleRespuestaChange(pregunta.id, value.toString())}
              min={1}
              max={5}
              step={1}
              marks={[
                { value: 1, label: '1' },
                { value: 2, label: '2' },
                { value: 3, label: '3' },
                { value: 4, label: '4' },
                { value: 5, label: '5' }
              ]}
              valueLabelDisplay="auto"
            />
            <Box display="flex" justifyContent="space-between" mt={1}>
              <Typography variant="caption">Muy Malo</Typography>
              <Typography variant="caption">Excelente</Typography>
            </Box>
          </Box>
        );

      default:
        return <Typography color="error">Tipo de pregunta no soportado</Typography>;
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  if (!encuesta) {
    return (
      <Container>
        <Alert severity="error">Encuesta no encontrada</Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          {encuesta.titulo}
        </Typography>
        
        {encuesta.descripcion && (
          <Typography variant="body1" color="textSecondary" paragraph>
            {encuesta.descripcion}
          </Typography>
        )}

        {error && (
          <Alert severity="error" sx={{ mb: 3 }}>
            {error}
          </Alert>
        )}

        {success && (
          <Alert severity="success" sx={{ mb: 3 }}>
            {success}
          </Alert>
        )}

        <form onSubmit={handleSubmit}>
          <Box sx={{ mt: 4 }}>
            {preguntas.map((pregunta, index) => (
              <Paper key={pregunta.id} sx={{ p: 3, mb: 3 }} variant="outlined">
                <Typography variant="h6" gutterBottom>
                  {index + 1}. {pregunta.enunciado}
                  {pregunta.obligatorio && (
                    <Chip 
                      label="Obligatorio" 
                      color="error" 
                      size="small" 
                      sx={{ ml: 1 }} 
                    />
                  )}
                </Typography>
                
                <Typography 
                  variant="caption" 
                  color="textSecondary" 
                  display="block" 
                  sx={{ mb: 2 }}
                >
                  Tipo: {pregunta.tipoPregunta}
                </Typography>

                {renderPregunta(pregunta)}
              </Paper>
            ))}
          </Box>

          <Box display="flex" gap={2} justifyContent="flex-end" sx={{ mt: 4 }}>
            <Button
              variant="outlined"
              onClick={() => navigate('/encuestas')}
              disabled={submitting}
            >
              Cancelar
            </Button>
            <Button
              type="submit"
              variant="contained"
              disabled={submitting}
              size="large"
            >
              {submitting ? <CircularProgress size={24} /> : 'Enviar Respuestas'}
            </Button>
          </Box>
        </form>
      </Paper>
    </Container>
  );
};

// Función temporal para obtener preguntas de ejemplo
const getPreguntasEjemplo = (encuestaId) => {
  return [
    {
      id: 1,
      enunciado: '¿Cómo calificarías nuestro servicio?',
      tipoPregunta: 'Escala',
      obligatorio: true,
      opciones: [
        { id: 1, label: '1 - Muy Malo', value: '1' },
        { id: 2, label: '2 - Malo', value: '2' },
        { id: 3, label: '3 - Regular', value: '3' },
        { id: 4, label: '4 - Bueno', value: '4' },
        { id: 5, label: '5 - Excelente', value: '5' }
      ]
    },
    {
      id: 2,
      enunciado: '¿Qué características te gustaría ver mejoradas?',
      tipoPregunta: 'Texto',
      obligatorio: false,
      opciones: []
    },
    {
      id: 3,
      enunciado: '¿Qué productos de nuestra empresa conoces?',
      tipoPregunta: 'OpcionMultiple',
      obligatorio: true,
      opciones: [
        { id: 6, label: 'Producto A', value: 'producto_a' },
        { id: 7, label: 'Producto B', value: 'producto_b' },
        { id: 8, label: 'Producto C', value: 'producto_c' },
        { id: 9, label: 'Ninguno', value: 'ninguno' }
      ]
    },
    {
      id: 4,
      enunciado: '¿Recomendarías nuestros servicios a otras personas?',
      tipoPregunta: 'SeleccionUnica',
      obligatorio: true,
      opciones: [
        { id: 10, label: 'Sí, definitivamente', value: 'si' },
        { id: 11, label: 'Probablemente', value: 'probablemente' },
        { id: 12, label: 'No estoy seguro', value: 'no_seguro' },
        { id: 13, label: 'Probablemente no', value: 'probablemente_no' },
        { id: 14, label: 'Definitivamente no', value: 'no' }
      ]
    }
  ];
};

export default ResponderEncuesta;