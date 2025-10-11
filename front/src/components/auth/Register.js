// src/components/auth/Register.js
import React, { useState } from 'react';
import { Box, TextField, Button, Typography, Alert } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { authService } from '../../services/api'; // Asegúrate de tener la función register en tu service

const Register = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    nombre: '',
    email: '',
    password: '',
    confirmarPassword: ''
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
        password: formData.password
      });
      navigate('/login'); // Redirige a login después del registro
    } catch (err) {
      setError(err.response?.data?.message || 'Error al registrar el usuario');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      justifyContent="center"
      minHeight="100vh"
    >
      <Typography variant="h4" mb={3}>
        Registro de Usuario
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Box component="form" onSubmit={handleSubmit} sx={{ width: '300px', display: 'flex', flexDirection: 'column', gap: 2 }}>
        <TextField
          label="Nombre"
          fullWidth
          required
          value={formData.nombre}
          onChange={(e) => setFormData({ ...formData, nombre: e.target.value })}
        />
        <TextField
          label="Correo Electrónico"
          type="email"
          fullWidth
          required
          value={formData.email}
          onChange={(e) => setFormData({ ...formData, email: e.target.value })}
        />
        <TextField
          label="Contraseña"
          type="password"
          fullWidth
          required
          value={formData.password}
          onChange={(e) => setFormData({ ...formData, password: e.target.value })}
        />
        <TextField
          label="Confirmar Contraseña"
          type="password"
          fullWidth
          required
          value={formData.confirmarPassword}
          onChange={(e) => setFormData({ ...formData, confirmarPassword: e.target.value })}
        />
        <Button type="submit" variant="contained" disabled={loading}>
          {loading ? 'Registrando...' : 'Registrar'}
        </Button>
      </Box>

      <Button sx={{ mt: 2 }} onClick={() => navigate('/login')}>
        ¿Ya tienes cuenta? Inicia sesión
      </Button>
    </Box>
  );
};

export default Register;
