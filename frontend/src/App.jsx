import { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

import Navbar from './components/Navbar';
import Home from './components/Home';
import ReservaPage from './components/ReservaPage';
import Sobre from './components/Sobre';

function App() {
  const [temaEscuro, setTemaEscuro] = useState(() => {
    const temaSalvo = localStorage.getItem('temaRestaurante');
    return temaSalvo ? JSON.parse(temaSalvo) : true;
  });

  useEffect(() => {
    localStorage.setItem('temaRestaurante', JSON.stringify(temaEscuro));
    if (temaEscuro) {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
  }, [temaEscuro]);

  const alternarTema = () => setTemaEscuro(!temaEscuro);

  return (
    <Router>
      <div className="min-h-screen font-sans transition-colors duration-300 bg-gray-50 text-gray-800 dark:bg-[#121212] dark:text-gray-100 flex flex-col">
        
        <Navbar temaEscuro={temaEscuro} alternarTema={alternarTema} />

        <main className="flex-grow p-4 md:p-8">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/reservar" element={<ReservaPage />} />
            <Route path="/sobre" element={<Sobre />} />
          </Routes>
        </main>

        <footer className="text-center p-6 text-sm text-gray-500 dark:text-gray-600 border-t border-gray-200 dark:border-gray-800">
          <p>© Gheizla Santos - 2025 - Desenvolvido com .NET e React</p>
        </footer>

      </div>
    </Router>
  );
}

export default App;