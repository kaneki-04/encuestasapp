import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  Container,
  CircularProgress,
  Alert,
  Card,
  CardContent,
  Grid,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip
} from '@mui/material';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  PieChart,
  Pie,
  Cell,
  ResponsiveContainer
} from 'recharts';
import { respuestasService, encuestasService } from '../../services/api';

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884D8'];

const EstadisticasEncuesta = () => {
  const { id } = useParams();
  const [estadisticas, setEstadisticas] = useState(null);
  const [encuesta, setEncuesta] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadEstadisticas();
  }, [id]);

  const loadEstadisticas = async () => {
    try {
      setLoading(true);
      const [encuestaData, respuestasData] = await Promise.all([
        encuestasService.getEncuesta(id),
        respuestasService.getRespuestasByEncuesta(id)
      ]);
      
      setEncuesta(encuestaData);
      setEstadisticas(respuestasData);
    } catch (error) {
      setError('Error al cargar las estadísticas');
    } finally {
      setLoading(false);
    }
  };

  const generarDatosGrafico = (pregunta) => {
    if (!estadisticas?.respuestas) return [];

    const conteo = {};
    estadisticas.respuestas.forEach(respuestaUsuario => {
      const respuestaPregunta = respuestaUsuario.respuestas.find(
        r => r.pregunta === pregunta.enunciado
      );
      if (respuestaPregunta) {
        const valor = respuestaPregunta.respuesta;
        conteo[valor] = (conteo[valor] || 0) + 1;
      }
    });

    return Object.entries(conteo).map(([name, value]) => ({ name, value }));
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Container>
        <Alert severity="error">{error}</Alert>
      </Container>
    );
  }

  if (!estadisticas) {
    return (
      <Container>
        <Alert severity="info">No hay datos estadísticos disponibles</Alert>
      </Container>
    );
  }

  const preguntasEjemplo = [
    {
      id: 1,
      enunciado: '¿Cómo calificarías nuestro servicio?',
      tipoPregunta: 'Escala'
    },
    {
      id: 2,
      enunciado: '¿Recomendarías nuestros servicios?',
      tipoPregunta: 'SeleccionUnica'
    }
  ];

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Estadísticas de Encuesta
      </Typography>

      <Card sx={{ mb: 4 }}>
        <CardContent>
          <Typography variant="h5" gutterBottom>
            {encuesta?.titulo || 'Encuesta'}
          </Typography>
          <Typography variant="body1" color="textSecondary" paragraph>
            {encuesta?.descripcion || 'Descripción no disponible'}
          </Typography>
          <Box display="flex" gap={2} flexWrap="wrap">
            <Chip 
              label={`${estadisticas.respuestas?.length || 0} Respuestas`} 
              color="primary" 
            />
            <Chip 
              label={`Estado: ${encuesta?.estado || 'Desconocido'}`} 
              variant="outlined" 
            />
          </Box>
        </CardContent>
      </Card>

      {/* Resumen de respuestas */}
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Distribución de Respuestas
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={[{name: 'Respuestas', value: estadisticas.respuestas?.length || 0}]}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="value" fill="#8884d8" />
              </BarChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Ejemplo de Gráfico de Pregunta
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={[{name: 'Sí', value: 65}, {name: 'No', value: 35}]}
                  cx="50%"
                  cy="50%"
                  outerRadius={80}
                  fill="#8884d8"
                  dataKey="value"
                  label
                >
                  <Cell fill={COLORS[0]} />
                  <Cell fill={COLORS[1]} />
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>
      </Grid>

      {/* Tabla de respuestas detalladas */}
      <Paper sx={{ mt: 4, p: 3 }}>
        <Typography variant="h6" gutterBottom>
          Respuestas Detalladas
        </Typography>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Usuario</TableCell>
                <TableCell>Fecha</TableCell>
                <TableCell>Pregunta</TableCell>
                <TableCell>Respuesta</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {estadisticas.respuestas?.map((respuestaUsuario, index) => (
                <React.Fragment key={index}>
                  {respuestaUsuario.respuestas.map((respuesta, respIndex) => (
                    <TableRow key={`${index}-${respIndex}`}>
                      {respIndex === 0 && (
                        <>
                          <TableCell rowSpan={respuestaUsuario.respuestas.length}>
                            {respuestaUsuario.usuario}
                          </TableCell>
                          <TableCell rowSpan={respuestaUsuario.respuestas.length}>
                            {new Date(respuestaUsuario.fecha).toLocaleDateString()}
                          </TableCell>
                        </>
                      )}
                      <TableCell>{respuesta.pregunta}</TableCell>
                      <TableCell>{respuesta.respuesta}</TableCell>
                    </TableRow>
                  ))}
                </React.Fragment>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>
    </Container>
  );
};

export default EstadisticasEncuesta;