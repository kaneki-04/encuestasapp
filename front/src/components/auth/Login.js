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
  InputAdornment,
  Fade,
  Avatar, // Agregado: Para el ícono principal
  Link, // Agregado: Para el enlace de "Olvidó su contraseña"
  Grid, // Agregado: Para manejar el layout de "Recordarme" y el enlace
} from '@mui/material';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import AccountCircle from '@mui/icons-material/AccountCircle';
import VpnKeyOutlinedIcon from '@mui/icons-material/VpnKeyOutlined'; // Icono para la contraseña
import { useAuth } from '../../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';

const Login = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
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
      // Simulación de una llamada de API
      await new Promise((resolve) => setTimeout(resolve, 1500)); 
      
      // Llamada a la función de login (manteniendo la lógica funcional)
      await login(formData); 
    } catch (err) {
      setError('Credenciales incorrectas. Por favor, verifica tu usuario y contraseña.');
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

  // --- Mejoras de Diseño (Estilos) ---
  const primaryGreen = '#2e7d32'; // Un verde más oscuro y corporativo (Green 800)
  const lightGreen = '#a5d6a7';  // Un verde más claro (Green 200)

  return (
    <Box
      sx={{
        minHeight: '100vh',
        // Fondo de gradiente suave y profesional
        background: `linear-gradient(135deg, ${lightGreen} 0%, ${primaryGreen} 100%)`, 
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        py: 4,
      }}
    >
      <Fade in timeout={700}>
        <Container component="main" maxWidth="xs">
          <Paper
            elevation={15} // Elevación más marcada
            sx={{
              p: { xs: 3, sm: 5 }, // Relleno adaptable
              borderRadius: 4,
              // Efecto "Glassmorphism" con más énfasis en la opacidad
              backdropFilter: 'blur(10px)',
              backgroundColor: 'rgba(255, 255, 255, 0.95)', 
              boxShadow: '0 10px 40px rgba(0, 0, 0, 0.35)', // Sombra más profunda
            }}
          >
            {/* Contenedor del ícono principal */}
            <Box sx={{ 
                display: 'flex', 
                flexDirection: 'column', 
                alignItems: 'center', 
                mb: 3 
            }}>
              <Avatar sx={{ 
                  m: 1, 
                  bgcolor: primaryGreen, // Color corporativo
                  width: 56, 
                  height: 56 
              }}>
                <LockOutlinedIcon fontSize="large" />
              </Avatar>
            </Box>

            {/* Título Principal */}
            <Typography
              variant="h5" // Reducción de h4 a h5 para una mejor jerarquía
              align="center"
              sx={{
                fontWeight: 700,
                color: primaryGreen, // Color sólido para mejor legibilidad
                mb: 0.5,
              }}
            >
              Gestor de EncuestApps
            </Typography>

            {/* Subtítulo */}
            <Typography
              variant="subtitle2" // Subtítulo más discreto
              align="center"
              color="text.secondary"
              sx={{ mb: 3 }}
            >
              Inicia sesión con tus credenciales
            </Typography>

            {error && (
              <Alert severity="error" sx={{ mb: 2, borderRadius: 2 }}>
                {error}
              </Alert>
            )}

            <Box component="form" onSubmit={handleSubmit}>
              {/* Campo de Usuario */}
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
                variant="outlined" // Usar 'outlined' para un look más moderno
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <AccountCircle sx={{ color: primaryGreen }} />
                    </InputAdornment>
                  ),
                }}
              />

              {/* Campo de Contraseña */}
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
                variant="outlined"
                sx={{ mb: 1 }} // Reducir margen inferior para el Grid
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <VpnKeyOutlinedIcon sx={{ color: primaryGreen }} />
                    </InputAdornment>
                  ),
                }}
              />
              
              {/* Opciones de "Recordarme" y "Olvidé mi contraseña" en una cuadrícula */}
              <Grid container justifyContent="space-between" alignItems="center">
                <Grid item>
                  <FormControlLabel
                    control={
                      <Checkbox
                        name="rememberMe"
                        sx={{
                          color: lightGreen,
                          '&.Mui-checked': { color: primaryGreen },
                        }}
                        checked={formData.rememberMe}
                        onChange={handleChange}
                      />
                    }
                    label="Recordarme"
                    sx={{ mt: 1 }}
                  />
                </Grid>
                <Grid item>
                    <Link 
                        href="#" 
                        variant="body2" 
                        color={primaryGreen}
                        sx={{ 
                            textDecoration: 'none', 
                            fontWeight: 'bold', 
                            '&:hover': { textDecoration: 'underline' } 
                        }}
                    >
                        ¿Olvidaste la contraseña?
                    </Link>
                </Grid>
              </Grid>


              {/* Botón de Iniciar Sesión */}
              <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{
                  mt: 4,
                  py: 1.4,
                  fontWeight: 'bold',
                  fontSize: '1rem',
                  borderRadius: 3,
                  textTransform: 'none',
                  // Gradiente más sutil y enfocado en los colores corporativos
                  background: `linear-gradient(90deg, ${lightGreen} 0%, ${primaryGreen} 100%)`, 
                  color: '#fff',
                  boxShadow: '0 6px 15px rgba(46, 125, 50, 0.4)', // Sombra con color del tema
                  transition: 'all 0.3s ease',
                  '&:hover': {
                    background: `linear-gradient(90deg, ${primaryGreen} 0%, #1b5e20 100%)`, // Oscurecer al pasar el mouse
                    transform: 'translateY(-2px)',
                    boxShadow: '0 8px 20px rgba(46, 125, 50, 0.6)', 
                  },
                }}
                disabled={loading}
              >
                {loading ? (
                  <CircularProgress size={26} sx={{ color: '#ffffff' }} />
                ) : (
                  'Iniciar Sesión'
                )}
              </Button>

              {/* Botón para registrarse */}
              <Button
                fullWidth
                variant="text" // Usar 'text' para hacerlo más un enlace y menos un botón de acción principal
                sx={{
                  py: 1.2,
                  fontWeight: 'bold',
                  fontSize: '0.95rem',
                  borderRadius: 3,
                  mt: 2,
                  textTransform: 'none',
                  color: primaryGreen, // Texto con color primario
                  '&:hover': {
                    backgroundColor: 'transparent',
                    textDecoration: 'underline', // Destacar al pasar el mouse
                  },
                }}
                onClick={() => navigate('/auth/register')}
              >
                ¿No tienes cuenta? **Regístrate aquí**
              </Button>
            </Box>
          </Paper>
        </Container>
      </Fade>
    </Box>
  );
};

export default Login;