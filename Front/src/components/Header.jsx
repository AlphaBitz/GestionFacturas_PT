import React from 'react';
import { FaFileInvoiceDollar } from "react-icons/fa6";

const Header = () => {
  return (
    <header className="bg-gradient-to-r from-blue-600 to-blue-800 text-white p-4 shadow-lg">
      <div className="container mx-auto flex justify-between items-center">
        <div className="flex items-center space-x-3">
          <FaFileInvoiceDollar className="text-2xl" />
          <h1 className="text-2xl font-bold">Gestión de Facturas</h1>
        </div>
        <div className="flex items-center space-x-4">
          <span className="text-sm bg-blue-500 px-3 py-1 rounded-full">v1.0</span>
        </div>
      </div>
    </header>
  );
};

export default Header;
