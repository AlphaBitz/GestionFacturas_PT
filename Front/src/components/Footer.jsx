import React from 'react';

const Footer = () => {
  return (
    <footer className="bg-gray-100 border-t border-gray-200 py-4">
      <div className="container mx-auto text-center">
        <p className="text-sm text-gray-600">
          © {new Date().getFullYear()} Sistema de Facturación · 
          <span className="text-blue-600 ml-1">Desarrollado para Evaluación Técnica</span>
        </p>
      </div>
    </footer>
  );
};

export default Footer;
