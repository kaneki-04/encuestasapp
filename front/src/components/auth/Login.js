// src/components/auth/Login.js
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
  Checkbox,
  FormControlLabel,
  InputAdornment, // Importado para el icono del campo
} from '@mui/material';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import AccountCircle from '@mui/icons-material/AccountCircle'; 
import { useAuth } from '../../contexts/AuthContext';

const Login = () => {
  const { login } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const [formData, setFormData] = useState({
    username: '',
    password: '',
    rememberMe: false,
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      await new Promise(resolve => setTimeout(resolve, 500));
      await login(formData);
    } catch (err) {
      setError('Usuario o contraseña incorrectos. Por favor, inténtalo de nuevo.');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };

  return (
    // Se usa maxWidth="xs" para un formulario más compacto y centrado
    <Container component="main" maxWidth="xs"> 
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
            sx={{ fontWeight: 'bold' }} // Título en negrita
            gutterBottom
          >
            Gestor de Encuestas
          </Typography>
          <Typography 
            component="h2" 
            variant="h6" 
            align="center" 
            color="text.secondary" // Usar texto secundario para el subtítulo
            gutterBottom 
            sx={{ mb: 3 }}
          >
            Iniciar Sesión
          </Typography>

          {error && (
            <Alert severity="error" sx={{ mb: 2, borderRadius: 2 }}>
              {error}
            </Alert>
          )}

          <Box component="form" onSubmit={handleSubmit} sx={{ mt: 1 }}>
            <TextField
              margin="normal"
              required
              fullWidth
              id="username"
              label="Nombre de Usuario"
              name="username"
              autoComplete="username"
              autoFocus
              value={formData.username}
              onChange={handleChange}
              // Añadir un icono al inicio del campo de texto
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <AccountCircle color="action" />
                  </InputAdornment>
                ),
              }}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              name="password"
              label="Contraseña"
              type="password"
              id="password"
              autoComplete="current-password"
              value={formData.password}
              onChange={handleChange}
              // Añadir un icono al inicio del campo de contraseña
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <LockOutlinedIcon color="action" />
                  </InputAdornment>
                ),
              }}
            />
            <FormControlLabel
              control={
                <Checkbox
                  name="rememberMe"
                  color="secondary" // Usar secondary para Checkbox
                  checked={formData.rememberMe}
                  onChange={handleChange}
                />
              }
              label="Recordarme"
              sx={{ mt: 1 }}
            />
            <Button
              type="submit"
              fullWidth
              // Cambiar a color="secondary" para un contraste más vibrante en dark mode
              variant="contained" 
              color="secondary" 
              sx={{ mt: 4, mb: 2, py: 1.5, fontWeight: 'bold' }} // Margen superior y padding aumentados
              disabled={loading}
            >
              {loading ? <CircularProgress size={24} color="inherit" /> : 'Iniciar Sesión'}
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
};

export default Login;