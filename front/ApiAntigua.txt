// src/services/api.js - CONECTADO A APIS REST DEL BACKEND
const API_BASE = '/api';

// SERVICIO DE AUTENTICACIÓN
export const authService = {
  login: async (credentials) => {
    try {
      console.log('🔗 Enviando login a:', `${API_BASE}/auth/login`);
      
      const response = await fetch(`${API_BASE}/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(credentials),
        credentials: 'include',
      });

      console.log('📡 Respuesta HTTP:', response.status, response.statusText);

      if (!response.ok) {
        throw new Error(`Error ${response.status}: ${response.statusText}`);
      }

      const data = await response.json();
      console.log('✅ Login exitoso:', data);
      
      if (data.success) {
        return data.user;
      } else {
        throw new Error(data.message || 'Error en el login');
      }
    } catch (error) {
      console.error('❌ Error en login:', error.message);
      throw error;
    }
  },

  getCurrentUser: async () => {
    try {
      const response = await fetch(`${API_BASE}/auth/current-user`, {
        credentials: 'include',
      });

      if (response.ok) {
        const user = await response.json();
        return user;
      }
      return null;
    } catch (error) {
      console.error('Error verificando usuario:', error);
      return null;
    }
  },

  register: async (userData) => {
    try {
      const response = await fetch(`${API_BASE}/auth/register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(userData),
        credentials: 'include',
      });

      const data = await response.json();

      if (response.ok && data.success) {
        return data.user;
      } else {
        throw new Error(data.message || 'Error en el registro');
      }
    } catch (error) {
      console.error('Error en registro:', error);
      throw error;
    }
  },

  logout: async () => {
    try {
      await fetch(`${API_BASE}/auth/logout`, {
        method: 'POST',
        credentials: 'include',
      });
    } catch (error) {
      console.error('Error en logout:', error);
    }
  }
};

// SERVICIO DE ENCUESTAS - USANDO APIS REST
export const encuestasService = {
  // Obtener todas las encuestas del usuario - API REST
  getEncuestas: async () => {
    try {
      console.log('📋 Obteniendo encuestas desde API REST...');
      
      const response = await fetch(`${API_BASE}/encuestas`, {
        credentials: 'include',
        headers: {
          'Accept': 'application/json',
        },
      });

      console.log('📡 Estado respuesta encuestas:', response.status);

      if (response.ok) {
        const encuestas = await response.json();
        console.log('✅ Encuestas obtenidas:', encuestas.length, 'encuestas');
        return encuestas;
      } else if (response.status === 404) {
        // Si la API no existe aún, usar datos de ejemplo
        console.log('⚠️ API no encontrada, usando datos de ejemplo');
        return getEncuestasEjemplo();
      } else {
        console.error('❌ Error obteniendo encuestas:', response.status);
        return getEncuestasEjemplo();
      }
    } catch (error) {
      console.error('❌ Error en getEncuestas:', error.message);
      return getEncuestasEjemplo();
    }
  },

  // Crear nueva encuesta - API REST
  createEncuesta: async (encuestaData) => {
    try {
      console.log('➕ Creando encuesta via API REST:', encuestaData);
      
      const response = await fetch(`${API_BASE}/encuestas`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(encuestaData),
        credentials: 'include',
      });

      console.log('📡 Respuesta crear encuesta:', response.status, response.statusText);

      if (response.ok) {
        const result = await response.json();
        console.log('✅ Encuesta creada exitosamente:', result);
        return { 
          success: true, 
          message: result.message || 'Encuesta creada exitosamente',
          id: result.id,
          redirectUrl: '/encuestas'
        };
      } else {
        // Si falla la API REST, intentar con el endpoint MVC
        console.log('⚠️ API REST falló, intentando con endpoint MVC...');
        return await createEncuestaMVC(encuestaData);
      }
    } catch (error) {
      console.error('❌ Error en createEncuesta (REST):', error);
      // Fallback a MVC
      return await createEncuestaMVC(encuestaData);
    }
  },

  // Obtener encuesta por ID - API REST
  getEncuesta: async (id) => {
    try {
      console.log('📖 Obteniendo encuesta ID:', id, 'via API REST');
      
      const response = await fetch(`${API_BASE}/encuestas/${id}`, {
        credentials: 'include',
        headers: {
          'Accept': 'application/json',
        },
      });

      if (response.ok) {
        const encuesta = await response.json();
        console.log('✅ Encuesta obtenida:', encuesta);
        return encuesta;
      } else if (response.status === 404) {
        console.log('⚠️ Encuesta no encontrada en API, usando datos de ejemplo');
        return getEncuestaEjemplo(id);
      } else {
        console.error('❌ Error obteniendo encuesta:', response.status);
        return getEncuestaEjemplo(id);
      }
    } catch (error) {
      console.error('❌ Error en getEncuesta:', error.message);
      return getEncuestaEjemplo(id);
    }
  },

  // Actualizar encuesta - API REST
  updateEncuesta: async (id, encuestaData) => {
    try {
      console.log('✏️ Actualizando encuesta ID:', id, 'via API REST:', encuestaData);
      
      const response = await fetch(`${API_BASE}/encuestas/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(encuestaData),
        credentials: 'include',
      });

      console.log('📡 Respuesta actualizar encuesta:', response.status, response.statusText);

      if (response.ok) {
        const result = await response.json();
        console.log('✅ Encuesta actualizada exitosamente:', result);
        return { 
          success: true, 
          message: result.message || 'Encuesta actualizada exitosamente',
          redirectUrl: '/encuestas'
        };
      } else {
        // Si falla la API REST, intentar con el endpoint MVC
        console.log('⚠️ API REST falló, intentando con endpoint MVC...');
        return await updateEncuestaMVC(id, encuestaData);
      }
    } catch (error) {
      console.error('❌ Error en updateEncuesta (REST):', error);
      // Fallback a MVC
      return await updateEncuestaMVC(id, encuestaData);
    }
  },

  // Eliminar encuesta - API REST
  deleteEncuesta: async (id) => {
    try {
      console.log('🗑️ Eliminando encuesta ID:', id, 'via API REST');
      
      const response = await fetch(`${API_BASE}/encuestas/${id}`, {
        method: 'DELETE',
        credentials: 'include',
      });

      console.log('📡 Respuesta eliminar encuesta:', response.status, response.statusText);

      if (response.ok) {
        const result = await response.json();
        console.log('✅ Encuesta eliminada exitosamente:', result);
        return { 
          success: true, 
          message: result.message || 'Encuesta eliminada exitosamente' 
        };
      } else {
        // Si falla la API REST, intentar con el endpoint MVC
        console.log('⚠️ API REST falló, intentando con endpoint MVC...');
        return await deleteEncuestaMVC(id);
      }
    } catch (error) {
      console.error('❌ Error en deleteEncuesta (REST):', error);
      // Fallback a MVC
      return await deleteEncuestaMVC(id);
    }
  }
};

// SERVICIO DE PREGUNTAS
export const preguntasService = {
  // Obtener preguntas de una encuesta
  getPreguntasByEncuesta: async (encuestaId) => {
    try {
      console.log('❓ Obteniendo preguntas para encuesta ID:', encuestaId);
      
      const response = await fetch(`${API_BASE}/preguntas/encuesta/${encuestaId}`, {
        credentials: 'include',
        headers: {
          'Accept': 'application/json',
        },
      });

      if (response.ok) {
        const preguntas = await response.json();
        console.log('✅ Preguntas obtenidas:', preguntas.length);
        return preguntas;
      } else {
        console.log('⚠️ API no disponible, usando datos de ejemplo');
        return getPreguntasEjemplo(encuestaId);
      }
    } catch (error) {
      console.error('❌ Error en getPreguntasByEncuesta:', error);
      return getPreguntasEjemplo(encuestaId);
    }
  },

  // Crear nueva pregunta
  createPregunta: async (encuestaId, preguntaData) => {
    try {
      console.log('➕ Creando pregunta para encuesta:', encuestaId, preguntaData);
      
      const preguntaRequest = {
        ...preguntaData,
        encuestaId: parseInt(encuestaId)
      };

      const response = await fetch(`${API_BASE}/preguntas`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(preguntaRequest),
        credentials: 'include',
      });

      if (response.ok) {
        const result = await response.json();
        console.log('✅ Pregunta creada:', result);
        return { success: true, message: 'Pregunta creada exitosamente', id: result.id };
      } else {
        return await createPreguntaMVC(encuestaId, preguntaData);
      }
    } catch (error) {
      console.error('❌ Error en createPregunta:', error);
      return await createPreguntaMVC(encuestaId, preguntaData);
    }
  },

  // Eliminar pregunta
  deletePregunta: async (preguntaId) => {
    try {
      console.log('🗑️ Eliminando pregunta ID:', preguntaId);
      
      const response = await fetch(`${API_BASE}/preguntas/${preguntaId}`, {
        method: 'DELETE',
        credentials: 'include',
      });

      if (response.ok) {
        return { success: true, message: 'Pregunta eliminada exitosamente' };
      } else {
        throw new Error('Error al eliminar la pregunta');
      }
    } catch (error) {
      console.error('❌ Error en deletePregunta:', error);
      throw error;
    }
  }
};

// SERVICIO DE RESPUESTAS
export const respuestasService = {
  // Responder una encuesta
  responderEncuesta: async (encuestaId, respuestas) => {
    try {
      console.log('📝 Respondiendo encuesta ID:', encuestaId, respuestas);
      
      const respuestaRequest = {
        encuestaId: parseInt(encuestaId),
        respuestas: respuestas
      };

      const response = await fetch(`${API_BASE}/respuestas/responder`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(respuestaRequest),
        credentials: 'include',
      });

      if (response.ok) {
        const result = await response.json();
        console.log('✅ Encuesta respondida:', result);
        return { success: true, message: 'Encuesta respondida exitosamente' };
      } else {
        return await responderEncuestaMVC(encuestaId, respuestas);
      }
    } catch (error) {
      console.error('❌ Error en responderEncuesta:', error);
      return await responderEncuestaMVC(encuestaId, respuestas);
    }
  },

  // Obtener respuestas del usuario
  getMisRespuestas: async () => {
    try {
      console.log('📊 Obteniendo mis respuestas...');
      
      const response = await fetch(`${API_BASE}/respuestas/mis-respuestas`, {
        credentials: 'include',
        headers: {
          'Accept': 'application/json',
        },
      });

      if (response.ok) {
        const respuestas = await response.json();
        console.log('✅ Respuestas obtenidas:', respuestas.length);
        return respuestas;
      } else {
        console.log('⚠️ API no disponible, usando datos de ejemplo');
        return getRespuestasEjemplo();
      }
    } catch (error) {
      console.error('❌ Error en getMisRespuestas:', error);
      return getRespuestasEjemplo();
    }
  },

  // Obtener respuestas de una encuesta (para el creador)
  getRespuestasByEncuesta: async (encuestaId) => {
    try {
      console.log('📈 Obteniendo respuestas de encuesta ID:', encuestaId);
      
      const response = await fetch(`${API_BASE}/respuestas/encuesta/${encuestaId}`, {
        credentials: 'include',
        headers: {
          'Accept': 'application/json',
        },
      });

      if (response.ok) {
        const respuestas = await response.json();
        console.log('✅ Respuestas de encuesta obtenidas:', respuestas.length);
        return respuestas;
      } else {
        console.log('⚠️ API no disponible, usando datos de ejemplo');
        return getRespuestasEncuestaEjemplo(encuestaId);
      }
    } catch (error) {
      console.error('❌ Error en getRespuestasByEncuesta:', error);
      return getRespuestasEncuestaEjemplo(encuestaId);
    }
  }
};

// ============================================================================
// MÉTODOS FALLBACK - USAN ENDPOINTS MVC EXISTENTES
// ============================================================================

// Crear encuesta usando endpoint MVC (fallback)
const createEncuestaMVC = async (encuestaData) => {
  try {
    console.log('🔄 Creando encuesta via MVC...');
    
    const formData = new FormData();
    formData.append('Titulo', encuestaData.titulo);
    formData.append('Descripcion', encuestaData.descripcion || '');
    formData.append('Estado', encuestaData.estado || 'Activa');
    formData.append('CierraEn', encuestaData.cierraEn);

    const response = await fetch('/Encuestas/CreateForm', {
      method: 'POST',
      body: formData,
      credentials: 'include',
    });

    console.log('📡 Respuesta MVC crear encuesta:', response.status);

    if (response.ok) {
      return { 
        success: true, 
        message: 'Encuesta creada exitosamente (MVC)',
        redirectUrl: '/encuestas'
      };
    } else {
      const errorText = await response.text();
      console.error('❌ Error del servidor MVC:', errorText);
      throw new Error('Error al crear la encuesta. Verifica los datos.');
    }
  } catch (error) {
    console.error('❌ Error en createEncuestaMVC:', error);
    throw error;
  }
};

// Actualizar encuesta usando endpoint MVC (fallback)
const updateEncuestaMVC = async (id, encuestaData) => {
  try {
    console.log('🔄 Actualizando encuesta via MVC...');
    
    const formData = new FormData();
    formData.append('Id', id);
    formData.append('Titulo', encuestaData.titulo);
    formData.append('Descripcion', encuestaData.descripcion || '');
    formData.append('Estado', encuestaData.estado);
    formData.append('CierraEn', encuestaData.cierraEn);
    formData.append('CreadoEn', encuestaData.creadoEn || new Date().toISOString());
    formData.append('AutorId', 1); // Temporal

    const response = await fetch(`/Encuestas/Edit/${id}`, {
      method: 'POST',
      body: formData,
      credentials: 'include',
    });

    console.log('📡 Respuesta MVC actualizar encuesta:', response.status);

    if (response.ok) {
      return { 
        success: true, 
        message: 'Encuesta actualizada exitosamente (MVC)',
        redirectUrl: '/encuestas'
      };
    } else {
      const errorText = await response.text();
      console.error('❌ Error del servidor MVC:', errorText);
      throw new Error('Error al actualizar la encuesta');
    }
  } catch (error) {
    console.error('❌ Error en updateEncuestaMVC:', error);
    throw error;
  }
};

// Eliminar encuesta usando endpoint MVC (fallback)
const deleteEncuestaMVC = async (id) => {
  try {
    console.log('🔄 Eliminando encuesta via MVC...');
    
    const response = await fetch(`/Encuestas/Delete/${id}`, {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: `id=${id}`
    });

    console.log('📡 Respuesta MVC eliminar encuesta:', response.status);

    if (response.ok) {
      return { 
        success: true, 
        message: 'Encuesta eliminada exitosamente (MVC)' 
      };
    } else {
      const errorText = await response.text();
      console.error('❌ Error del servidor MVC:', errorText);
      throw new Error('Error al eliminar la encuesta');
    }
  } catch (error) {
    console.error('❌ Error en deleteEncuestaMVC:', error);
    throw error;
  }
};

// Métodos fallback para preguntas y respuestas
const createPreguntaMVC = async (encuestaId, preguntaData) => {
  try {
    console.log('🔄 Creando pregunta via MVC...');
    
    const formData = new FormData();
    formData.append('Enunciado', preguntaData.enunciado);
    formData.append('TipoPregunta', preguntaData.tipoPregunta);
    formData.append('Obligatorio', preguntaData.obligatorio.toString());
    formData.append('encuestaId', encuestaId);

    const response = await fetch(`/Encuestas/CreatePregunta?encuestaId=${encuestaId}`, {
      method: 'POST',
      body: formData,
      credentials: 'include',
    });

    if (response.ok) {
      return { success: true, message: 'Pregunta creada exitosamente (MVC)' };
    } else {
      throw new Error('Error al crear la pregunta');
    }
  } catch (error) {
    console.error('❌ Error en createPreguntaMVC:', error);
    throw error;
  }
};

const responderEncuestaMVC = async (encuestaId, respuestas) => {
  try {
    console.log('🔄 Respondiendo encuesta via MVC...');
    
    const formData = new FormData();
    formData.append('EncuestaId', encuestaId);
    
    respuestas.forEach((respuesta, index) => {
      formData.append(`Preguntas[${index}].PreguntaId`, respuesta.preguntaId);
      formData.append(`Preguntas[${index}].RespuestaTexto`, respuesta.respuestaTexto || '');
      formData.append(`Preguntas[${index}].RespuestaOpcionId`, respuesta.respuestaOpcionId || '');
    });

    const response = await fetch('/Respuestas/Responder', {
      method: 'POST',
      body: formData,
      credentials: 'include',
    });

    if (response.ok) {
      return { success: true, message: 'Encuesta respondida exitosamente (MVC)' };
    } else {
      throw new Error('Error al responder la encuesta');
    }
  } catch (error) {
    console.error('❌ Error en responderEncuestaMVC:', error);
    throw error;
  }
};

// ============================================================================
// DATOS DE EJEMPLO (para desarrollo/testing)
// ============================================================================

// Datos de ejemplo para cuando la API no esté disponible
const getEncuestasEjemplo = () => {
  return [
    {
      id: 1,
      titulo: 'Encuesta de Satisfacción - Ejemplo API',
      descripcion: 'Esta encuesta usa la API REST del backend',
      estado: 'Activa',
      cierraEn: '2024-12-31',
      creadoEn: '2024-01-15',
      totalRespuestas: 15
    },
    {
      id: 2,
      titulo: 'Preferencias de Producto - Ejemplo API',
      descripcion: 'Encuesta sobre preferencias de nuevos productos usando API REST',
      estado: 'Inactiva',
      cierraEn: '2024-06-30',
      creadoEn: '2024-01-10',
      totalRespuestas: 8
    },
    {
      id: 3,
      titulo: 'Opinión sobre Servicios - Ejemplo API',
      descripcion: 'Recopilamos tu opinión sobre nuestros servicios actuales',
      estado: 'Activa',
      cierraEn: '2024-11-15',
      creadoEn: '2024-02-01',
      totalRespuestas: 23
    }
  ];
};

// Encuesta individual de ejemplo
const getEncuestaEjemplo = (id) => {
  return {
    id: parseInt(id),
    titulo: 'Encuesta de Ejemplo - API REST',
    descripcion: 'Esta encuesta se obtiene desde la API REST del backend',
    estado: 'Activa',
    cierraEn: '2024-12-31',
    creadoEn: '2024-01-15',
    totalRespuestas: 15
  };
};

// Datos de ejemplo para preguntas y respuestas
const getPreguntasEjemplo = (encuestaId) => {
  const preguntasBase = [
    {
      id: 1,
      enunciado: '¿Cómo calificarías nuestro servicio?',
      tipoPregunta: 'Escala',
      obligatorio: true,
      opciones: [
        { id: 1, label: '1 - Muy Malo', value: '1' },
        { id: 2, label: '2 - Malo', value: '2' },
        { id: 3, label: '3 - Regular', value: '3' },
        { id: 4, label: '4 - Bueno', value: '4' },
        { id: 5, label: '5 - Excelente', value: '5' }
      ]
    },
    {
      id: 2,
      enunciado: '¿Qué características te gustaría ver mejoradas?',
      tipoPregunta: 'Texto',
      obligatorio: false,
      opciones: []
    },
    {
      id: 3,
      enunciado: '¿Qué productos usas regularmente?',
      tipoPregunta: 'OpcionMultiple',
      obligatorio: true,
      opciones: [
        { id: 6, label: 'Producto A', value: 'producto_a' },
        { id: 7, label: 'Producto B', value: 'producto_b' },
        { id: 8, label: 'Producto C', value: 'producto_c' },
        { id: 9, label: 'Ninguno', value: 'ninguno' }
      ]
    },
    {
      id: 4,
      enunciado: '¿Recomendarías nuestros servicios?',
      tipoPregunta: 'SeleccionUnica',
      obligatorio: true,
      opciones: [
        { id: 10, label: 'Sí', value: 'si' },
        { id: 11, label: 'No', value: 'no' },
        { id: 12, label: 'Tal vez', value: 'tal_vez' }
      ]
    }
  ];

  return preguntasBase.map(p => ({
    ...p,
    encuestaId: parseInt(encuestaId)
  }));
};

const getRespuestasEjemplo = () => {
  return [
    {
      id: 1,
      encuesta: { titulo: 'Encuesta de Satisfacción' },
      pregunta: { enunciado: '¿Cómo calificarías nuestro servicio?' },
      texto: '5 - Excelente',
      fechaRespuesta: '2024-01-20T10:30:00'
    },
    {
      id: 2,
      encuesta: { titulo: 'Encuesta de Satisfacción' },
      pregunta: { enunciado: '¿Qué características te gustaría ver mejoradas?' },
      texto: 'Mejorar el tiempo de respuesta',
      fechaRespuesta: '2024-01-20T10:30:00'
    },
    {
      id: 3,
      encuesta: { titulo: 'Preferencias de Producto' },
      pregunta: { enunciado: '¿Qué productos usas regularmente?' },
      texto: 'Producto A, Producto B',
      fechaRespuesta: '2024-01-18T14:20:00'
    }
  ];
};

const getRespuestasEncuestaEjemplo = (encuestaId) => {
  return {
    encuesta: {
      id: parseInt(encuestaId),
      titulo: 'Encuesta de Satisfacción - Ejemplo',
      descripcion: 'Respuestas de ejemplo para esta encuesta'
    },
    respuestas: [
      {
        usuario: 'usuario1',
        fecha: '2024-01-20T10:30:00',
        respuestas: [
          { pregunta: '¿Cómo calificarías nuestro servicio?', respuesta: '5 - Excelente' },
          { pregunta: '¿Qué características mejorar?', respuesta: 'Tiempo de respuesta' },
          { pregunta: '¿Qué productos usas?', respuesta: 'Producto A, Producto B' }
        ]
      },
      {
        usuario: 'usuario2',
        fecha: '2024-01-19T15:45:00',
        respuestas: [
          { pregunta: '¿Cómo calificarías nuestro servicio?', respuesta: '4 - Bueno' },
          { pregunta: '¿Qué características mejorar?', respuesta: 'Soporte técnico' },
          { pregunta: '¿Qué productos usas?', respuesta: 'Producto C' }
        ]
      }
    ]
  };
};