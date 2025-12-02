export default function Sobre() {
  return (
    <div className="max-w-3xl mx-auto animate-fade-in">
      <div className="bg-white dark:bg-[#1e1e1e] rounded-2xl shadow-lg border border-gray-100 dark:border-gray-800 p-8">
        <h1 className="text-3xl font-bold text-brand mb-6">Sobre o Projeto</h1>
        
        <div className="space-y-6 text-gray-700 dark:text-gray-300 leading-relaxed">
          <p>
            Este projeto é uma solução <strong>Full-Stack</strong> desenvolvida para aperfeiçoar competências em arquitetura de software e experiência do usuário.
          </p>

          <hr className="border-gray-200 dark:border-gray-700" />

          <h2 className="text-xl font-bold text-gray-900 dark:text-white">🛠️ Stack Tecnológica</h2>
          <ul className="list-disc pl-5 space-y-2">
            <li><strong>Front-End:</strong> React.js, Vite, Tailwind CSS (Design Responsivo & Dark Mode).</li>
            <li><strong>Back-End:</strong> .NET 10 (C#), Entity Framework Core.</li>
            <li><strong>Banco de Dados:</strong> PostgreSQL com índices otimizados.</li>
            <li><strong>Arquitetura:</strong> RESTful API, Transações ACID para evitar Overbooking.</li>
          </ul>

          <hr className="border-gray-200 dark:border-gray-700" />

          <h2 className="text-xl font-bold text-gray-900 dark:text-white">💡 Destaque: Prevenção de Overbooking</h2>
          <p>
            O diferencial deste sistema é a lógica de <strong>concorrência</strong>. Utilizando transações de banco de dados e bloqueio otimista, o sistema garante que duas pessoas nunca consigam reservar a mesma mesa no mesmo horário, mesmo que cliquem no botão no exato mesmo milissegundo.
          </p>
        </div>
      </div>
    </div>
  );
}