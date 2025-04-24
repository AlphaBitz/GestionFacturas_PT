# Proyecto de Práctica Técnica
## Tecnologías Utilizadas
- Backend: .NET 8, Entity Framework Core, SQLite
- Frontend: React, TailwindCSS, MUI Data Grid
- Patrón de Diseño: Repository Pattern + Unit of Work

## Instalación y Ejecución

### Backend
1. Clonar el repositorio
2. Navegar a la carpeta `Back
3. Ejecutar `dotnet restore`
4. Ejecutar `dotnet run`
5. La API estará disponible en `http://localhost:5142`
6. Documentación Swagger en `http://localhost:5142/swagger`

### Frontend
1. Navegar a la carpeta `Front`
2. Ejecutar `npm install`
3. Ejecutar `npm start`
4. La aplicación estará disponible en `http://localhost:3000`

## Configuración HTTPS
Se ha decidido deshabilitar HTTPS redirection para simplificar la evaluación.
En un entorno productivo, se recomienda:
- Habilitar `app.UseHttpsRedirection()`
- Configurar certificados válidos
- Usar siempre HTTPS en producción

### Consideraciones Adicionales
- Para mantener la seguridad, se agregaron las comprobaciones al agregar una NC mayoritariamente en el back.
- Se habia implementado una condición para que no se pudieran actualizar si la factura estaba fuera de plazo,
    pero esta fue eliminada debido a la cantidad de solicitudes con este estado en el JSON.
- Si se desea automatizar el estado de pago de las facturas se puede implementar un trigger en la bdd,
  que al final de cada dia actualize los valores, en esta implementación no esta agregado. 
- Si la factura presenta un rut invalido tambien sera eliminada.
