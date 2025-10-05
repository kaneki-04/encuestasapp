// src/App.js - Agrega los nuevos imports y rutas
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { Box, CircularProgress } from '@mui/material';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import Login from './components/auth/Login';
import EncuestasList from './components/encuestas/EncuestasList';
import CreateEncuesta from './components/encuestas/CreateEncuesta';
import EditEncuesta from './components/encuestas/EditEncuesta';

const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
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

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Router>
      <Routes>
        <Route 
          path="/login" 
          element={!isAuthenticated ? <Login /> : <Navigate to="/encuestas" />} 
        />
        <Route 
          path="/encuestas" 
          element={
            <ProtectedRoute>
              <EncuestasList />
            </ProtectedRoute>
          } 
        />
        <Route 
          path="/encuestas/create" 
          element={
            <ProtectedRoute>
              <CreateEncuesta />
            </ProtectedRoute>
          } 
        />
        <Route 
          path="/encuestas/edit/:id" 
          element={
            <ProtectedRoute>
              <EditEncuesta />
            </ProtectedRoute>
          } 
        />
        <Route 
          path="/" 
          element={<Navigate to={isAuthenticated ? "/encuestas" : "/login"} />} 
        />
      </Routes>
    </Router>
  );
}

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <AuthProvider>
        <AppContent />
      </AuthProvider>
    </ThemeProvider>
  );
}

export default App;