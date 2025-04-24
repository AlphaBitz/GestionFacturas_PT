import React, { useState } from 'react';
import { Modal, Box, Button, TextField, CircularProgress } from '@mui/material';
import { FaTimes } from 'react-icons/fa';
import { toast } from 'react-toastify';
import { sendCreditNote } from '../services/invoiceservices'; // Ajusta la ruta según tu estructura

const CreditNoteModal = ({ open, handleClose, invoice, refreshData  }) => {
  const [creditNoteData, setCreditNoteData] = useState({
    id: '',
    value: ''
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState({});

  const validateForm = () => {
    const newErrors = {};
    
    if (!creditNoteData.id.trim()) {
      newErrors.id = 'El ID de la nota de crédito es requerido';
    }
    
    if (!creditNoteData.value) {
      newErrors.value = 'El valor es requerido';
    } else if (isNaN(parseFloat(creditNoteData.value))) {
      newErrors.value = 'Debe ser un valor numérico';
    } else if (parseFloat(creditNoteData.value) <= 0) {
      newErrors.value = 'El valor debe ser mayor a cero';
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setCreditNoteData(prev => ({
      ...prev,
      [name]: value
    }));

    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const handleSubmit = async () => {
    if (!validateForm()) return;
    
    setIsSubmitting(true);
    
    try {
      const payload = {
        invoice_number: invoice?.invoiceNumber,
        credit_note_amount: parseFloat(creditNoteData.value),
        credit_note_number: creditNoteData.id.trim()
      };

      console.log('Payload a enviar:', JSON.stringify(payload, null, 2));
      console.log('Detalles completos:', {
        payload,
        invoiceData: invoice,
        timestamp: new Date().toISOString()
      });

      const result = await sendCreditNote(payload);
      toast.success(result.message || 'Nota de crédito creada exitosamente');
      refreshData();
      setCreditNoteData({ id: '', value: '' });
      handleClose();
      
    } catch (error) {
      console.error('Error en la solicitud:', {
        error: error.message,
        details	: error.details	,
        stack: error.stack,
        timestamp: new Date().toISOString()
      });
      console.log(error);
      toast.error(error.details);
    } finally {
      setIsSubmitting(false);
    }
  };

  const modalStyle = {
    position: 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    width: 400,
    bgcolor: 'background.paper',
    boxShadow: 24,
    p: 4,
    borderRadius: 2,
  };

  return (
    <Modal
      open={open}
      onClose={handleClose}
      aria-labelledby="credit-note-modal"
      aria-describedby="credit-note-modal-description"
    >
      <Box sx={modalStyle}>
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Agregar Nota de Crédito</h2>
          <button 
            onClick={handleClose} 
            className="text-gray-500 hover:text-gray-700"
            disabled={isSubmitting}
          >
            <FaTimes />
          </button>
        </div>
        
        {/* Información de la factura */}
        <div className="mb-4">
          <p className="text-lg mb-2">Factura N°: <span className="font-bold">{invoice?.invoiceNumber}</span></p>
          <p className="text-lg mb-2">Monto Total: <span className="font-bold">${invoice?.totalAmount}</span></p>
          <p className="text-lg mb-2">Monto Pendiente: <span className="font-bold">${invoice?.montoPendiente}</span></p>
        </div>

        <div className="space-y-4 mb-6">
          <TextField
            fullWidth
            label="Insertar ID Nota Crédito"
            variant="outlined"
            name="id"
            value={creditNoteData.id}
            onChange={handleChange}
            size="small"
            error={!!errors.id}
            helperText={errors.id}
            disabled={isSubmitting}
          />
          
          <TextField
            fullWidth
            label="Insertar Valor NC"
            variant="outlined"
            name="value"
            type="number"
            value={creditNoteData.value}
            onChange={handleChange}
            size="small"
            InputProps={{
              startAdornment: <span className="mr-2">$</span>,
            }}
            error={!!errors.value}
            helperText={errors.value}
            disabled={isSubmitting}
          />
        </div>
        
        <div className="flex justify-end gap-3 mt-6">
          <Button 
            variant="outlined" 
            onClick={handleClose}
            sx={{ textTransform: 'none' }}
            disabled={isSubmitting}
          >
            Cancelar
          </Button>
          <Button 
            variant="contained" 
            onClick={handleSubmit}
            sx={{ textTransform: 'none' }}
            disabled={isSubmitting}
            startIcon={isSubmitting ? <CircularProgress size={20} /> : null}
          >
            {isSubmitting ? 'Enviando...' : 'Enviar Nota de Crédito'}
          </Button>
        </div>
      </Box>
    </Modal>
  );
};

export default CreditNoteModal;
