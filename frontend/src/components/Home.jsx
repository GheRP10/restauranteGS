import { Link } from 'react-router-dom';

export default function Home() {
  return (
    <div className="max-w-4xl mx-auto animate-fade-in">
      
      <div className="bg-white dark:bg-[#1e1e1e] rounded-2xl shadow-xl border border-gray-100 dark:border-gray-800 p-8 md:p-12 text-center mb-10">
        <h2 className="text-sm font-bold tracking-widest text-brand uppercase mb-3">
          Projeto Full-Stack
        </h2>
        <h1 className="text-3xl md:text-5xl font-bold text-gray-900 dark:text-white mb-6">
          Sistema de Reservas Inteligente
        </h1>
        <p className="text-lg text-gray-600 dark:text-gray-400 mb-8 max-w-2xl mx-auto">
          Um sistema para gerenciamento de mesas, com verificação de disponibilidade em tempo real e prevenção de overbooking.
        </p>
        
        <div className="flex flex-col sm:flex-row gap-4 justify-center">
          <Link 
            to="/reservar"
            className="px-8 py-3 bg-brand hover:bg-brand-hover text-white font-bold rounded-full shadow-lg hover:shadow-brand/50 transition-all transform hover:-translate-y-1"
          >
            🚀 Fazer uma Reserva
          </Link>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-white dark:bg-[#1e1e1e] p-6 rounded-xl shadow-md border border-gray-100 dark:border-gray-800">
          <div className="text-3xl mb-2">⚡</div>
          <h3 className="font-bold text-gray-900 dark:text-white">Performance</h3>
          <p className="text-sm text-gray-500 dark:text-gray-400">Back-end em .NET 10 otimizado</p>
        </div>
        <div className="bg-white dark:bg-[#1e1e1e] p-6 rounded-xl shadow-md border border-gray-100 dark:border-gray-800">
          <div className="text-3xl mb-2">🔒</div>
          <h3 className="font-bold text-gray-900 dark:text-white">Segurança</h3>
          <p className="text-sm text-gray-500 dark:text-gray-400">Transações atômicas no banco</p>
        </div>
        <div className="bg-white dark:bg-[#1e1e1e] p-6 rounded-xl shadow-md border border-gray-100 dark:border-gray-800">
          <div className="text-3xl mb-2">🎨</div>
          <h3 className="font-bold text-gray-900 dark:text-white">Moderno</h3>
          <p className="text-sm text-gray-500 dark:text-gray-400">Front em React + Tailwind</p>
        </div>
      </div>
    </div>
  );
}