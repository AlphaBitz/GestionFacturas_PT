export const readJsonFile = (file) =>
    new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = (event) => {
        try {
          const parsed = JSON.parse(event.target.result);
          resolve(parsed);
        } catch (err) {
          reject(new Error('El archivo no es un JSON válido.'));
        }
      };
      reader.readAsText(file);
    });
  