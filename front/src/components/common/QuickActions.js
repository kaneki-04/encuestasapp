import React from 'react';
import { useNavigate } from 'react-router-dom';
import { SpeedDial, SpeedDialAction, SpeedDialIcon } from '@mui/material';
import {
  Add as AddIcon,
  QuestionAnswer as PreguntasIcon,
  List as RespuestasIcon,
  Home as HomeIcon // <-- Icono cambiado de StatsIcon a HomeIcon
} from '@mui/icons-material';

const QuickActions = () => {
  const navigate = useNavigate();

  const actions = [
    { icon: <AddIcon />, name: 'Nueva Encuesta', action: () => navigate('/encuestas/create') },
    { icon: <PreguntasIcon />, name: 'Mis Respuestas', action: () => navigate('/mis-respuestas') },
    { icon: <HomeIcon />, name: 'Inicio', action: () => navigate('/encuestas') },
  ];

  return (
    <SpeedDial
      ariaLabel="Acciones rápidas"
      sx={{ position: 'fixed', bottom: 18, right: 18 }}
      icon={<SpeedDialIcon />}
      FabProps={{
        sx: {
          bgcolor: '#19d272ff',       // Color principal del botón +
          color: '#000000ff',            // Color del icono "+"
          '&:hover': {
            bgcolor: '#119345ff',     // Color al pasar el mouse
          },
        },
      }}
    >
      {actions.map((action) => (
        <SpeedDialAction
          key={action.name}
          icon={action.icon}
          tooltipTitle={action.name}
          onClick={action.action}
        />
      ))}
    </SpeedDial>
  );
};

export default QuickActions;
