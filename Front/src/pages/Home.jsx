import React, { useState } from 'react';
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { sendInvoicesToApi } from '../services/invoiceservices';
import { readJsonFile } from '../utils/fileReader';
import DataGridInvoices from '../components/DataGridInvoices';

const Home = () => {
  const [jsonData, setJsonData] = useState(null);
  const [isSending, setIsSending] = useState(false);

  const handleFileUpload = async (e) => {
    const file = e.target.files[0]; // Obtiene el archivo cargado por el usuario.
    if (!file) return;

    try {
      const parsed = await readJsonFile(file); // Lee el archivo JSON y lo parsea.
      setJsonData(parsed);
      if (!parsed?.invoices) {
        toast.error("No hay facturas válidas para enviar.");
        return;
      }

      setIsSending(true);
      const { status, result } = await sendInvoicesToApi(parsed);

      if (status === 201) {
        toast.success(result.message, {
          onClose: () => window.location.reload() // Recarga al cerrar el toast de éxito
        });
      } else if (status === 207) {
        toast.warn(result.message, {
          onClose: () => window.location.reload() // Recarga al cerrar el toast de advertencia
        });
      } 
      else toast.error(result.error || 'Error desconocido');

    } catch (err) {
      toast.error(err.message);
    } finally {
      setIsSending(false);
    }
  };

  const invoiceRows = jsonData?.invoices?.map((inv, index) => ({
    id: index + 1, // Establece un ID único para cada factura (basado en el índice).
    invoiceNumber: inv.invoiceNumber || `N/A`, // Si no existe el número de factura, muestra 'N/A'.
    amount: inv.amount || 0, // Si no existe el monto, muestra 0.
    status: inv.status || 'Desconocido' // Si no existe el estado, muestra 'Desconocido'.
  })) || [];

  return (
    <div className="max-w-6xl mx-auto">
      <div className="bg-white rounded-xl shadow-md p-6 mb-6">
        <h2 className="text-2xl font-semibold text-gray-800 mb-6">Panel Principal</h2>
        
        <div className="flex flex-col items-center mb-8">
          <label className="inline-flex items-center px-6 py-3 bg-blue-600 text-white font-medium rounded-lg shadow-md hover:bg-blue-700 transition duration-300 cursor-pointer">
            <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
            </svg>
            Cargar archivo JSON
            <input
              type="file"
              accept=".json"
              onChange={handleFileUpload}
              className="hidden"
              disabled={isSending}
            />
          </label>
          {isSending && (
            <p className="mt-2 text-sm text-gray-500 flex items-center">
              <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-blue-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Procesando archivo...
            </p>
          )}
        </div>

        <div className="border rounded-lg over">
          <DataGridInvoices rows={invoiceRows} />
        </div>
      </div>
    </div>
  );
};

export default Home;
