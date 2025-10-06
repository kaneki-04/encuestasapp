import React from 'react';
import { useNavigate } from 'react-router-dom';
import { SpeedDial, SpeedDialAction, SpeedDialIcon } from '@mui/material';
import {
  Add as AddIcon,
  QuestionAnswer as PreguntasIcon,
  List as RespuestasIcon,
  BarChart as StatsIcon
} from '@mui/icons-material';

const QuickActions = () => {
  const navigate = useNavigate();

  const actions = [
    { icon: <AddIcon />, name: 'Nueva Encuesta', action: () => navigate('/encuestas/create') },
    { icon: <PreguntasIcon />, name: 'Mis Respuestas', action: () => navigate('/mis-respuestas') },
    { icon: <StatsIcon />, name: 'Estadísticas', action: () => navigate('/encuestas') },
  ];

  return (
    <SpeedDial
      ariaLabel="Acciones rápidas"
      sx={{ position: 'fixed', bottom: 16, right: 16 }}
      icon={<SpeedDialIcon />}
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