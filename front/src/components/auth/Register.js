// src/components/auth/Register.js
import React, { useState } from 'react';
import { Box, TextField, Button, Typography, Alert, CircularProgress } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { authService } from '../../services/api'; // Asegúrate de tener la función register en tu service

const Register = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    nombre: '',
    email: '',
    password: '',
    confirmarPassword: '',
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (formData.password !== formData.confirmarPassword) {
      setError('Las contraseñas no coinciden');
      return;
    }

    try {
      setLoading(true);
      setError('');
      await authService.register({
        nombre: formData.nombre,
        email: formData.email,
        password: formData.password,
      });
      navigate('/login'); // Redirige a login después del registro
    } catch (err) {
      setError(err.response?.data?.message || 'Error al registrar el usuario');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  return (
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      justifyContent="center"
      minHeight="100vh"
      sx={{ px: 2 }}
    >
      <Typography
        variant="h4"
        mb={3}
        sx={{
          fontWeight: 700,
          background: 'linear-gradient(90deg, #009f5d, #3cb371)',
          WebkitBackgroundClip: 'text',
          WebkitTextFillColor: 'transparent',
        }}
      >
        Registro de Usuario
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 2, width: '300px' }}>{error}</Alert>}

      <Box
        component="form"
        onSubmit={handleSubmit}
        sx={{ width: '300px', display: 'flex', flexDirection: 'column', gap: 2 }}
      >
        <TextField
          label="Nombre"
          name="nombre"
          fullWidth
          required
          value={formData.nombre}
          onChange={handleChange}
        />
        <TextField
          label="Correo Electrónico"
          name="email"
          type="email"
          fullWidth
          required
          value={formData.email}
          onChange={handleChange}
        />
        <TextField
          label="Contraseña"
          name="password"
          type="password"
          fullWidth
          required
          value={formData.password}
          onChange={handleChange}
        />
        <TextField
          label="Confirmar Contraseña"
          name="confirmarPassword"
          type="password"
          fullWidth
          required
          value={formData.confirmarPassword}
          onChange={handleChange}
        />
        <Button
          type="submit"
          variant="contained"
          fullWidth
          disabled={loading}
          sx={{
            mt: 1,
            py: 1.5,
            fontWeight: 'bold',
            background: 'linear-gradient(90deg, #68efad 0%, #3cb371 100%)',
            color: '#fff',
            '&:hover': {
              background: 'linear-gradient(90deg, #4be38f 0%, #009f5d 100%)',
            },
          }}
        >
          {loading ? <CircularProgress size={24} sx={{ color: '#fff' }} /> : 'Registrar'}
        </Button>
      </Box>

      <Button
        sx={{ mt: 2, textTransform: 'none' }}
        onClick={() => navigate('/login')}
      >
        ¿Ya tienes cuenta? Inicia sesión
      </Button>
    </Box>
  );
};

export default Register;
