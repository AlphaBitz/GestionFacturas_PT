// Envía un lote de facturas a la API
export const sendInvoicesToApi = async (jsonData) => {
  try {
    const response = await fetch('http://localhost:5142/api/invoice/batch', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(jsonData)
    });

    const result = await response.json();
    return { status: response.status, result };
  } catch (err) {
    throw new Error("Error de conexión con la API.");
  }
};

// Obtiene resúmenes de facturas desde la API
export const getInvoiceSummaries = async () => {
  try {
    const response = await fetch('http://localhost:5142/api/invoice/summaries');

    if (!response.ok) {
      throw new Error('Error al obtener los datos de las facturas');
    }

    const data = await response.json();
    return data;
  } catch (error) {
    throw error;
  }
};

// Envía una nota de crédito a la API
export const sendCreditNote = async (payload) => {
  const apiUrl = process.env.REACT_APP_API_URL || 'http://localhost:5142/api';

  const response = await fetch(`${apiUrl}/invoice/credit-note`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`,
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    const errorData = await response.json();
    const error = new Error(errorData.message || `Error ${response.status}: ${response.statusText}`);
    error.details = errorData.details; 
    throw error;
  }

  return await response.json();
};