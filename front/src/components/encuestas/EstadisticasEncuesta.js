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
  Chip,
  Tooltip as MuiTooltip,
  Divider
} from '@mui/material';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  PieChart,
  Pie,
  Cell,
  ResponsiveContainer
} from 'recharts';
import { respuestasService, encuestasService } from '../../services/api';

// Colores modernos para las gráficas
const COLORS = ['#42a5f5', '#66bb6a', '#ffa726', '#ef5350', '#ab47bc'];

const EstadisticasEncuesta = () => {
  const { id } = useParams();
  const [estadisticas, setEstadisticas] = useState(null);
  const [encuesta, setEncuesta] = useState(null);
  const [graficos, setGraficos] = useState([]);
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

      // 🧠 Generar estadísticas por pregunta
      const graficosData = {};
      respuestasData.respuestas?.forEach((usuario) => {
        usuario.respuestas.forEach((r) => {
          const preguntaEnunciado = r.pregunta; 
          const respuestaValor = r.respuesta;

          if (!graficosData[preguntaEnunciado]) graficosData[preguntaEnunciado] = {};
          
          graficosData[preguntaEnunciado][respuestaValor] =
            (graficosData[preguntaEnunciado][respuestaValor] || 0) + 1;
        });
      });

      // Convertir a formato de Recharts (usando graficosMap)
      const graficosMap = Object.keys(graficosData).map((pregunta) => ({
        pregunta,
        data: Object.entries(graficosData[pregunta]).map(([respuesta, count]) => ({
          name: respuesta,
          value: count,
        })),
      }));

      // ⚙️ LÓGICA DE ORDENACIÓN (Para mantener el orden de la encuesta en los gráficos)
      const graficosPorEnunciado = graficosMap.reduce((acc, curr) => {
          acc[curr.pregunta] = curr;
          return acc;
      }, {});
      
      const preguntasOrdenadas = encuestaData.preguntas || [];
      
      const graficosFinalesOrdenados = preguntasOrdenadas
          .map(p => {
              return graficosPorEnunciado[p.enunciado]; 
          })
          .filter(grafico => grafico); 
          
      setGraficos(graficosFinalesOrdenados.length > 0 ? graficosFinalesOrdenados : graficosMap); 

    } catch (e) {
      console.error(e);
      setError('Error al cargar las estadísticas. Revise la consola para detalles. Asegúrese de que su API de encuestas devuelva la lista de preguntas.');
    } finally {
      setLoading(false);
    }
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
        <Alert severity="error" sx={{ borderRadius: 2, mb: 2 }}>{error}</Alert>
      </Container>
    );
  }

  if (!estadisticas || !estadisticas.respuestas?.length) {
    return (
      <Container>
        <Alert severity="info" sx={{ borderRadius: 2, mb: 2 }}>
          No hay respuestas registradas para esta encuesta.
        </Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      {/* 1. ENCABEZADO DE LA ENCUESTA */}
      <Paper elevation={4} sx={{ p: 4, borderRadius: 3, mb: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom sx={{ fontWeight: 'bold' }}>
          {encuesta?.titulo || 'Estadísticas de Encuesta'}
        </Typography>
        <Typography variant="body1" color="text.secondary" paragraph>
          {encuesta?.descripcion || 'Análisis de las respuestas recolectadas.'}
        </Typography>
        <Divider sx={{ my: 2 }} />
        <Box display="flex" gap={2} flexWrap="wrap" alignItems="center">
          <Chip label={`${estadisticas.respuestas?.length} respuestas`} color="primary" variant="filled" />
          <Chip label={`Estado: ${encuesta?.estado}`} variant="outlined" />
        </Box>
      </Paper>

      {/* 2. RESPUESTAS DETALLADAS (MOVIDA ARRIBA) */}
      <Typography variant="h5" gutterBottom sx={{ mt: 5, mb: 3, fontWeight: 'bold' }}>
        Respuestas Detalladas:
      </Typography>

      <Paper sx={{ mt: 1, p: 2, borderRadius: 3, boxShadow: 6, mb: 5 }}>
        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow sx={{ bgcolor: '#f5f5f5' }}>
                <TableCell sx={{ fontWeight: 'bold' }}>Usuario</TableCell>
                <TableCell sx={{ fontWeight: 'bold' }}>Fecha</TableCell>
                <TableCell sx={{ fontWeight: 'bold' }}>Pregunta</TableCell>
                <TableCell sx={{ fontWeight: 'bold' }}>Respuesta</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {estadisticas.respuestas.map((respuestaUsuario, index) =>
                respuestaUsuario.respuestas.map((respuesta, i) => (
                  <TableRow key={`${index}-${i}`} hover>
                    {i === 0 && (
                      <>
                        <TableCell rowSpan={respuestaUsuario.respuestas.length}>
                          {respuestaUsuario.usuario || `Usuario ${index + 1}`}
                        </TableCell>
                        <TableCell rowSpan={respuestaUsuario.respuestas.length}>
                          {new Date(respuestaUsuario.fecha).toLocaleDateString()}
                        </TableCell>
                      </>
                    )}
                    <TableCell>{respuesta.pregunta}</TableCell>
                    <TableCell>
                        <MuiTooltip title={respuesta.respuesta} placement="top">
                            <Box component="span" sx={{ 
                                display: 'inline-block', 
                                maxWidth: '250px', 
                                overflow: 'hidden', 
                                textOverflow: 'ellipsis', 
                                whiteSpace: 'nowrap' 
                            }}>
                                {respuesta.respuesta}
                            </Box>
                        </MuiTooltip>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      {/* 3. GRÁFICAS POR PREGUNTA (MOVIDA ABAJO) */}
      <Typography variant="h5" gutterBottom sx={{ mt: 5, mb: 3, fontWeight: 'bold' }}>
        Gráficos:
      </Typography>

      <Grid container spacing={4}>
        {graficos.map((grafico, index) => {
            const isSingleResponse = grafico.data.length === 1;

            return (
              <Grid item xs={12} md={6} key={index}>
                <Card sx={{ borderRadius: 3, boxShadow: 6 }}>
                  <CardContent sx={{ p: 3 }}>
                    <Typography variant="h6" gutterBottom sx={{ fontWeight: 'bold', mb: 2 }}>
                      {grafico.pregunta}
                    </Typography>
                    <ResponsiveContainer width="100%" height={300}>
                      {/* Usar Gráfico de Barras para > 2 opciones o si es una sola respuesta */}
                      {grafico.data.length > 2 || isSingleResponse ? (
                        <BarChart data={grafico.data} margin={{ top: 20, right: 20, left: 10, bottom: 5 }}>
                          <CartesianGrid strokeDasharray="3 3" />
                          <XAxis 
                            dataKey="name" 
                            interval={0} 
                            angle={grafico.data.length > 3 ? -30 : 0} 
                            textAnchor={grafico.data.length > 3 ? "end" : "middle"} 
                            height={grafico.data.length > 3 ? 60 : 30} 
                          />
                          <YAxis allowDecimals={false} label={{ value: 'Frecuencia', angle: -90, position: 'insideLeft' }} />
                          <Tooltip />
                          <Bar dataKey="value" fill={COLORS[0]} radius={[6, 6, 0, 0]} />
                        </BarChart>
                      ) : (
                        // Usar Gráfico de Torta solo para 2 opciones
                        <PieChart>
                          <Pie
                            data={grafico.data}
                            cx="50%"
                            cy="50%"
                            outerRadius={110}
                            fill="#8884d8"
                            dataKey="value"
                            label={!isSingleResponse ? ({ name, percent }) => `${name} (${(percent * 100).toFixed(0)}%)` : false}
                          >
                            {grafico.data.map((_, i) => (
                              <Cell key={i} fill={COLORS[i % COLORS.length]} />
                            ))}
                          </Pie>
                          <Tooltip formatter={(value) => [`${value} respuestas`, 'Total']} />
                        </PieChart>
                      )}
                    </ResponsiveContainer>
                  </CardContent>
                </Card>
              </Grid>
            );
        })}
      </Grid>
    </Container>
  );
};

export default EstadisticasEncuesta;