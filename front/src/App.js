import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate, useLocation } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { Box, CircularProgress } from '@mui/material';
import { AuthProvider, useAuth } from './contexts/AuthContext';

// Componentes de Autenticación
import Login from './components/auth/Login';
import Register from './components/auth/Register';

// Componentes de Encuestas
import EncuestasList from './components/encuestas/EncuestasList';
import CreateEncuesta from './components/encuestas/CreateEncuesta';
import EditEncuesta from './components/encuestas/EditEncuesta';
import PreguntasManager from './components/encuestas/PreguntasManager';
import EstadisticasEncuesta from './components/encuestas/EstadisticasEncuesta';

// Componentes de Respuestas
import ResponderEncuesta from './components/respuestas/ResponderEncuesta';
import MisRespuestas from './components/respuestas/MisRespuestas';

// Componentes Comunes
import QuickActions from './components/common/QuickActions';

const theme = createTheme({
  palette: {
    primary: { main: '#1976d2' },
    secondary: { main: '#dc004e' },
  },
});

const ProtectedRoute = ({ children }) => {
  const { isAuthenticated, loading } = useAuth();
  
  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }
  
  return isAuthenticated ? children : <Navigate to="/login" />;
};

function AppContent() {
  const { isAuthenticated, loading } = useAuth();
  const location = useLocation();

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  const isLoginPage = location.pathname === '/login' || location.pathname === '/register';

  return (
    <>
      <Routes>
        {/* Autenticación */}
        <Route 
          path="/login" 
          element={!isAuthenticated ? <Login /> : <Navigate to="/encuestas" />} 
        />
        <Route
          path="/register"
          element={!isAuthenticated ? <Register /> : <Navigate to="/encuestas" />}
        />

        {/* Encuestas */}
        <Route 
          path="/encuestas" 
          element={<ProtectedRoute><EncuestasList /></ProtectedRoute>} 
        />
        <Route 
          path="/encuestas/create" 
          element={<ProtectedRoute><CreateEncuesta /></ProtectedRoute>} 
        />
        <Route 
          path="/encuestas/edit/:id" 
          element={<ProtectedRoute><EditEncuesta /></ProtectedRoute>} 
        />
        <Route 
          path="/encuestas/:id/preguntas" 
          element={<ProtectedRoute><PreguntasManager /></ProtectedRoute>} 
        />
        <Route 
          path="/encuestas/:id/estadisticas" 
          element={<ProtectedRoute><EstadisticasEncuesta /></ProtectedRoute>} 
        />

        {/* Respuestas */}
        <Route 
          path="/encuestas/:id/responder" 
          element={<ProtectedRoute><ResponderEncuesta /></ProtectedRoute>} 
        />
        <Route 
          path="/mis-respuestas" 
          element={<ProtectedRoute><MisRespuestas /></ProtectedRoute>} 
        />

        {/* Ruta por defecto */}
        <Route 
          path="/" 
          element={<Navigate to={isAuthenticated ? "/encuestas" : "/login"} />} 
        />
      </Routes>

      {/* QuickActions solo si no estamos en login o registro */}
      {!isLoginPage && <QuickActions />}
    </>
  );
}

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <AuthProvider>
        <Router>
          <AppContent />
        </Router>
      </AuthProvider>
    </ThemeProvider>
  );
}

export default App;
