import { Link } from 'react-router-dom';

export default function Navbar({ temaEscuro, alternarTema }) {
  return (
    <nav className="bg-white dark:bg-[#1e1e1e] border-b border-gray-200 dark:border-gray-800 sticky top-0 z-50 transition-colors duration-300">
      <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16 items-center">
          
          <div className="flex-shrink-0 flex items-center gap-2">
            <span className="text-2xl">🍽️</span>
            <span className="font-bold text-xl tracking-tight text-gray-800 dark:text-white">
              Reserva<span className="text-brand">Fácil GS</span>
            </span>
          </div>

          <div className="hidden md:flex space-x-8">
            <Link to="/" className="text-gray-600 dark:text-gray-300 hover:text-brand dark:hover:text-brand font-medium transition-colors">
              Início
            </Link>
            <Link to="/reservar" className="text-gray-600 dark:text-gray-300 hover:text-brand dark:hover:text-brand font-medium transition-colors">
              Reservar
            </Link>
            <Link to="/sobre" className="text-gray-600 dark:text-gray-300 hover:text-brand dark:hover:text-brand font-medium transition-colors">
              Sobre
            </Link>
          </div>

          <div>
            <button 
              onClick={alternarTema}
              className="p-2 rounded-full bg-gray-100 dark:bg-gray-800 text-gray-600 dark:text-yellow-400 hover:bg-gray-200 dark:hover:bg-gray-700 transition-all"
              title={temaEscuro ? "Mudar para Claro" : "Mudar para Escuro"}
            >
              {temaEscuro ? 'Modo claro 🌞' : 'Modo escuro 🌙'}
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
}