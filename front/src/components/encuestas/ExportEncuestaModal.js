import React, { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  CircularProgress,
} from '@mui/material';
import { encuestasService } from '../../services/api';

const ExportEncuestaModal = ({ open, onClose }) => {
  const [encuestas, setEncuestas] = useState([]);
  const [selectedEncuesta, setSelectedEncuesta] = useState(null);
  const [loading, setLoading] = useState(false);
  const [downloading, setDownloading] = useState(false);

  useEffect(() => {
    if (open) loadEncuestas();
  }, [open]);

  const loadEncuestas = async () => {
    setLoading(true);
    try {
      const data = await encuestasService.getEncuestas();
      setEncuestas(data);
    } catch (error) {
      console.error('Error al cargar encuestas:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSelectChange = (event) => {
    const id = event.target.value;
    const encuesta = encuestas.find((e) => e.id === id);
    setSelectedEncuesta(encuesta);
  };

  const handleDownloadExcel = async () => {
    if (!selectedEncuesta) return;
    setDownloading(true);
    try {
      const response = await fetch(`http://localhost:5204/api/export/encuesta/${selectedEncuesta.id}/excel`);
      if (!response.ok) throw new Error('Error al descargar el Excel');

      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `Encuesta_${selectedEncuesta.titulo}.xlsx`;
      a.click();
      window.URL.revokeObjectURL(url);
    } catch (error) {
      alert('Error al exportar la encuesta.');
    } finally {
      setDownloading(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle> Exportar Encuesta a Excel</DialogTitle>
      <DialogContent dividers>
        {loading ? (
          <Box display="flex" justifyContent="center" p={3}>
            <CircularProgress />
          </Box>
        ) : (
          <>
            <FormControl fullWidth sx={{ mb: 3 }}>
              <InputLabel>Selecciona una encuesta</InputLabel>
              <Select value={selectedEncuesta?.id || ''} onChange={handleSelectChange}>
                {encuestas.map((encuesta) => (
                  <MenuItem key={encuesta.id} value={encuesta.id}>
                    {encuesta.titulo}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

            {selectedEncuesta && (
              <Box sx={{ p: 2, border: '1px solid #ddd', borderRadius: 2 }}>
                <Typography variant="subtitle1"><strong>ID:</strong> {selectedEncuesta.id}</Typography>
                <Typography variant="subtitle1"><strong>Título:</strong> {selectedEncuesta.titulo}</Typography>
                <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                  {selectedEncuesta.descripcion || 'Sin descripción'}
                </Typography>
                <Typography variant="body2" sx={{ mt: 1 }}>
                  <strong>Cierra:</strong> {new Date(selectedEncuesta.cierraEn).toLocaleDateString()}
                </Typography>
                <Typography variant="body2">
                  <strong>Estado:</strong> {selectedEncuesta.estado}
                </Typography>
              </Box>
            )}
          </>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} color="inherit">Cancelar</Button>
        <Button
          variant="contained"
          color="success"
          onClick={handleDownloadExcel}
          disabled={!selectedEncuesta || downloading}
        >
          {downloading ? 'Descargando...' : 'Descargar Excel'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default ExportEncuestaModal;
