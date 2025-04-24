import React, { useCallback, useEffect, useState } from 'react';
import { DataGrid } from '@mui/x-data-grid';
import { esES } from '@mui/x-data-grid/locales';
import { toast } from 'react-toastify';
import { getInvoiceSummaries } from '../services/invoiceservices';
import { FaPlus } from 'react-icons/fa';
import { TextField } from '@mui/material';
import CreditNoteModal from './ModalNC';
import { format } from 'date-fns';
// Definición de columnas de la tabla con estilos y render personalizado para el botón de "Agregar NC"
const columns = [
  { field: 'invoiceNumber', headerName: 'N° Factura', flex: 1, headerClassName: 'super-app-theme--header', headerAlign: 'center', align: 'center' },
  { field: 'customerRun', headerName: 'RUN Cliente', flex: 1, headerClassName: 'super-app-theme--header', headerAlign: 'center', align: 'center' },
  { field: 'customerName', headerName: 'Nombre Cliente', flex: 0.7, headerClassName: 'super-app-theme--header', headerAlign: 'center', align: 'center' },
  { field: 'daysToDue', headerName: 'Días Venc.', type: 'number', flex: 0.7, headerClassName: 'super-app-theme--header', headerAlign: 'center', align: 'center' },
  { field: 'invoiceStatus', headerName: 'Est. Factura', flex: 1, headerClassName: 'super-app-theme--header', headerAlign: 'center', align: 'center' },
  { field: 'paymentStatus', headerName: 'Est. Pago', flex: 1, headerClassName: 'super-app-theme--header', headerAlign: 'center', align: 'center' },
  { field: 'totalAmount', headerName: 'Monto Total', type: 'number', flex: 1, headerClassName: 'super-app-theme--header', headerAlign: 'center', align: 'center' },
  {
    field: 'invoiceDate',
    headerName: 'Fecha',
    flex: 1,
    headerClassName: 'super-app-theme--header',
    headerAlign: 'center',
    align: 'center',
    valueFormatter: (params) => {
      try {
        if (!params) return '';
        const rawDate = new Date(params);
        if (isNaN(rawDate)) return '';
        return format(rawDate, 'dd-MM-yyyy');
      } catch {
        return '';
      }
    },
  },  {
    field: 'creditNoteAmount',
    headerName: 'Monto Nota Crédito',
    type: 'number',
    headerClassName: 'super-app-theme--header',
    headerAlign: 'center',
    align: 'center',
    flex: 1,
  },
  {
    field: 'actions',
    headerName: 'Agregar',
    headerClassName: 'super-app-theme--header',
    headerAlign: 'center',
    align: 'center',
    sortable: false,
    filterable: false,
    disableColumnMenu: true,
    flex: 1,
    renderCell: (params) => (
      <div className="w-full h-full flex items-center justify-center">
        <button
          onClick={() => params.row.handleOpenModal()}
          className="flex items-center gap-2 bg-blue-600 text-white font-bold px-2 py-2 rounded hover:bg-blue-700 transition-colors text-sm"
        >
          <FaPlus />
          Agregar NC
        </button>
      </div>
    ),
    
  }
];

const InvoiceDataGrid = () => {
  const [rows, setRows] = useState([]); // Lista completa de facturas
  const [filteredRows, setFilteredRows] = useState([]); // Lista filtrada para búsqueda
  const [searchTerm, setSearchTerm] = useState(''); // Texto ingresado para búsqueda
  const [openModal, setOpenModal] = useState(false); // Controla visibilidad del modal
  const [currentInvoice, setCurrentInvoice] = useState(null); // Factura actual para el modal

  // Función para obtener los datos actualizados desde la API
  const fetchSummaries = useCallback(async () => { // Agregar 'async' aquí
    try {
      const data = await getInvoiceSummaries(); // Llama al servicio que obtiene los resúmenes de facturas
      const formattedData = data.map((item, index) => ({
        id: index + 1,
        ...item,
        handleOpenModal: () => handleOpenModal(item),
      }));
      setRows(formattedData);
      setFilteredRows(formattedData);
    } catch (error) {
      toast.error(error.message || 'Error al cargar los datos');
    }
  }, []);

  // Abre el modal y setea la factura seleccionada
  const handleOpenModal = (row) => {
    const montoPendiente = row.totalAmount - row.creditNoteAmount; // Calcula el monto pendiente
    console.log(row);
    console.log(montoPendiente);
    setCurrentInvoice({
      ...row,
      montoPendiente, // Agrega "Monto Pendiente" al objeto de la factura
    });
    setOpenModal(true);
  };

  // Cierra el modal
  const handleCloseModal = () => {
    setOpenModal(false);
  };


  
  useEffect(() => {
    fetchSummaries();
  }, [fetchSummaries]);

  // Filtra las facturas por número o estado
  const handleSearch = (searchTerm) => {
    const lowerSearchTerm = searchTerm.toLowerCase();
    const filtered = rows.filter((row) => {
      const invoiceStr = String(row.invoiceNumber || '').toLowerCase();
      const statusStr = String(row.invoiceStatus || '').toLowerCase();
      return (
        invoiceStr.includes(lowerSearchTerm) ||
        statusStr.includes(lowerSearchTerm)
      );
    });
    setFilteredRows(filtered);
  };

  return (
    <div style={{ height: 630, display: 'flex', flexDirection: 'column' }}>
      {/* Input para búsqueda */}
      <div className="flex justify-end mb-4" style={{ flexShrink: 0, padding: '0 16px' }}>
        <TextField
          label="Buscar por N° Factura o Estado"
          variant="outlined"
          size="small"
          value={searchTerm}
          onChange={(e) => {
            const value = e.target.value;
            setSearchTerm(value);
            handleSearch(value);
          }}
          style={{ width: 300 }}
        />
      </div>

      {/* Tabla de facturas */}
      <DataGrid
        rows={filteredRows}
        columns={columns}
        autoHeight={true}
        initialState={{
          pagination: { paginationModel: { pageSize: 10 } },
        }}
        pageSizeOptions={[10, 25, 50]}
        columnVisibilityModel={{ creditNoteAmount: false }}
        localeText={esES.components.MuiDataGrid.defaultProps.localeText}
        getRowClassName={(params) => {
          const isEven = params.indexRelativeToCurrentPage % 2 === 0;
          return isEven ? 'table-row-even' : 'table-row-odd';
        }}
        sx={{
          width: '100%',
          '& .MuiDataGrid-cell:hover': {
            color: 'primary.main',
          },
          '& .super-app-theme--header': {
            fontSize: 13,
            backgroundColor: '#3f4994',
            color: 'white',
          },
          '& .table-row-even .MuiDataGrid-cell': {
            backgroundColor: '#EAF9FF',
          },
        }}
      />

      {/* Modal para agregar Nota de Crédito */}
      <CreditNoteModal 
        open={openModal} 
        handleClose={handleCloseModal} 
        invoice={currentInvoice} 
        refreshData={fetchSummaries} // Pasar la función para refrescar los datos
      />
    </div>
  );
};

export default InvoiceDataGrid;

