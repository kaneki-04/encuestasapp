import React from 'react';
import { useNavigate } from 'react-router-dom';
import { SpeedDial, SpeedDialAction, SpeedDialIcon } from '@mui/material';
import {
  Add as AddIcon,
  QuestionAnswer as PreguntasIcon,
  List as RespuestasIcon,
  Home as HomeIcon 
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
          bgcolor: '#21b669ff',       
          color: '#000000ff',           
          '&:hover': {
            bgcolor: '#0b8055ff',     
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
