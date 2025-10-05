import React, { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  Button,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  IconButton,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControlLabel,
  Checkbox,
  MenuItem,
  Alert,
  CircularProgress,
  Divider
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
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
  const [editingPregunta, setEditingPregunta] = useState(null);

  const [formData, setFormData] = useState({
    enunciado: '',
    tipoPregunta: 'Texto',
    obligatorio: false,
    opciones: []
  });

  useEffect(() => {
    loadPreguntas();
  }, [id]);

  const loadPreguntas = async () => {
    try {
      setLoading(true);
      const data = await preguntasService.getPreguntasByEncuesta(id);
      setPreguntas(data);
    } catch (error) {
      setError('Error al cargar las preguntas');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    setEditingPregunta(null);
    setFormData({
      enunciado: '',
      tipoPregunta: 'Texto',
      obligatorio: false,
      opciones: []
    });
    setOpenDialog(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      setError('');
      await preguntasService.createPregunta(id, formData);
      setOpenDialog(false);
      await loadPreguntas();
    } catch (error) {
      setError('Error al crear la pregunta: ' + error.message);
    }
  };

  const handleDelete = async (preguntaId) => {
    if (window.confirm('¿Estás seguro de que quieres eliminar esta pregunta?')) {
      try {
        await preguntasService.deletePregunta(preguntaId);
        await loadPreguntas();
      } catch (error) {
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

  const getTipoColor = (tipo) => {
    switch (tipo) {
      case 'SeleccionUnica': return 'primary';
      case 'OpcionMultiple': return 'secondary';
      case 'Texto': return 'success';
      case 'Escala': return 'warning';
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
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h5" component="h2">
          Preguntas de la Encuesta
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleCreate}
        >
          Agregar Pregunta
        </Button>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Paper elevation={2}>
        {preguntas.length === 0 ? (
          <Box textAlign="center" py={4}>
            <Typography variant="h6" color="textSecondary" gutterBottom>
              No hay preguntas en esta encuesta
            </Typography>
            <Typography variant="body2" color="textSecondary" sx={{ mb: 2 }}>
              Agrega la primera pregunta para comenzar
            </Typography>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={handleCreate}
            >
              Agregar Primera Pregunta
            </Button>
          </Box>
        ) : (
          <List>
            {preguntas.map((pregunta, index) => (
              <React.Fragment key={pregunta.id}>
                <ListItem>
                  <Box display="flex" alignItems="flex-start" width="100%">
                    <Box mr={2} mt={0.5}>
                      {getTipoIcon(pregunta.tipoPregunta)}
                    </Box>
                    <Box flex={1}>
                      <ListItemText
                        primary={
                          <Box display="flex" alignItems="center" gap={1}>
                            <Typography variant="subtitle1">
                              {pregunta.enunciado}
                            </Typography>
                            {pregunta.obligatorio && (
                              <Chip label="Obligatorio" color="error" size="small" />
                            )}
                          </Box>
                        }
                        secondary={
                          <Box mt={1}>
                            <Chip 
                              icon={getTipoIcon(pregunta.tipoPregunta)}
                              label={pregunta.tipoPregunta}
                              color={getTipoColor(pregunta.tipoPregunta)}
                              size="small"
                              variant="outlined"
                            />
                            {pregunta.opciones && pregunta.opciones.length > 0 && (
                              <Box mt={1}>
                                <Typography variant="caption" color="textSecondary">
                                  Opciones: {pregunta.opciones.map(o => o.label).join(', ')}
                                </Typography>
                              </Box>
                            )}
                          </Box>
                        }
                      />
                    </Box>
                    <ListItemSecondaryAction>
                      <IconButton 
                        edge="end" 
                        color="error"
                        onClick={() => handleDelete(pregunta.id)}
                      >
                        <DeleteIcon />
                      </IconButton>
                    </ListItemSecondaryAction>
                  </Box>
                </ListItem>
                {index < preguntas.length - 1 && <Divider />}
              </React.Fragment>
            ))}
          </List>
        )}
      </Paper>

      {/* Dialog para crear/editar pregunta */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="md" fullWidth>
        <DialogTitle>
          {editingPregunta ? 'Editar Pregunta' : 'Agregar Nueva Pregunta'}
        </DialogTitle>
        <form onSubmit={handleSubmit}>
          <DialogContent>
            <TextField
              autoFocus
              margin="dense"
              label="Enunciado de la pregunta"
              fullWidth
              variant="outlined"
              required
              value={formData.enunciado}
              onChange={(e) => setFormData({ ...formData, enunciado: e.target.value })}
              sx={{ mb: 2 }}
            />
            
            <TextField
              select
              margin="dense"
              label="Tipo de Pregunta"
              fullWidth
              variant="outlined"
              value={formData.tipoPregunta}
              onChange={(e) => setFormData({ ...formData, tipoPregunta: e.target.value })}
              sx={{ mb: 2 }}
            >
              <MenuItem value="Texto">
                <Box display="flex" alignItems="center" gap={1}>
                  <TextoIcon fontSize="small" />
                  Texto Libre
                </Box>
              </MenuItem>
              <MenuItem value="SeleccionUnica">
                <Box display="flex" alignItems="center" gap={1}>
                  <OpcionUnicaIcon fontSize="small" />
                  Selección Única
                </Box>
              </MenuItem>
              <MenuItem value="OpcionMultiple">
                <Box display="flex" alignItems="center" gap={1}>
                  <OpcionMultipleIcon fontSize="small" />
                  Opción Múltiple
                </Box>
              </MenuItem>
              <MenuItem value="Escala">
                <Box display="flex" alignItems="center" gap={1}>
                  <EscalaIcon fontSize="small" />
                  Escala Numérica
                </Box>
              </MenuItem>
            </TextField>

            <FormControlLabel
              control={
                <Checkbox
                  checked={formData.obligatorio}
                  onChange={(e) => setFormData({ ...formData, obligatorio: e.target.checked })}
                />
              }
              label="Pregunta obligatoria"
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setOpenDialog(false)}>Cancelar</Button>
            <Button type="submit" variant="contained">
              {editingPregunta ? 'Actualizar' : 'Crear'} Pregunta
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  );
};

export default PreguntasManager;