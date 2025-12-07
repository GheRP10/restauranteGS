import { useState, useEffect } from 'react';
import axios from 'axios';

// URL base da API vindo do ambiente Vite (Railway/Dev)
const API_URL = import.meta.env.VITE_API_URL;

export default function ReservaPage() {
  const [data, setData] = useState('');
  const [hora, setHora] = useState('');
  const [pessoas, setPessoas] = useState(2);
  const [mesasDisponiveis, setMesasDisponiveis] = useState([]);

  const [carregando, setCarregando] = useState(false);
  const [erro, setErro] = useState('');
  const [sucesso, setSucesso] = useState('');
  const [buscaRealizada, setBuscaRealizada] = useState(false);

  const [temaEscuro, setTemaEscuro] = useState(() => {
    const temaSalvo = localStorage.getItem('temaRestaurante');
    return temaSalvo ? JSON.parse(temaSalvo) : true;
  });

  useEffect(() => {
    localStorage.setItem('temaRestaurante', JSON.stringify(temaEscuro));
  }, [temaEscuro]);

  const alternarTema = () => setTemaEscuro(!temaEscuro);

  const normalizarListaMesas = (data) => {

    console.log('Resposta bruta de /mesas/disponibilidade:', data);

    if (Array.isArray(data)) {
      return data;
    }

    if (data && Array.isArray(data.mesas)) {
      return data.mesas;
    }

    return [];
  };

  const buscarMesas = async (e) => {
    e.preventDefault();
    setCarregando(true);
    setErro('');
    setSucesso('');
    setMesasDisponiveis([]);
    setBuscaRealizada(false);

    try {
      const url = `${API_URL}/api/mesas/disponibilidade`;
      console.log('Chamando API de disponibilidade em:', url);

      const response = await axios.get(url, {
        params: {
          dataHora: `${data}T${hora}:00`,
          numeroPessoas: pessoas
        }
      });

      const lista = normalizarListaMesas(response.data);
      setMesasDisponiveis(lista);
      setBuscaRealizada(true);
    } catch (error) {
      console.error('Erro ao buscar mesas:', error);
      const status = error.response?.status;
      if (status) {
        setErro(`Erro ao conectar com a API (status ${status}).`);
      } else {
        setErro('Erro ao conectar com a API. Verifique o Back-end.');
      }
    } finally {
      setCarregando(false);
    }
  };

  const reservarMesa = async (mesaId) => {
    if (!confirm("Deseja confirmar a reserva desta mesa?")) return;

    setCarregando(true);
    setErro('');
    setSucesso('');

    try {
      const payload = {
        mesaId: mesaId,
        usuarioId: 1,
        dataHoraInicio: `${data}T${hora}:00`,
        numeroPessoas: parseInt(pessoas)
      };

      const url = `${API_URL}/api/reservas`;
      console.log('Enviando reserva para:', url, payload);

      await axios.post(url, payload);

      setSucesso('✅ Reserva realizada com sucesso! Te esperamos lá.');
      setMesasDisponiveis([]);
      setBuscaRealizada(false);
    } catch (error) {
      console.error('Erro ao reservar mesa:', error);
      const msg = error.response?.data || 'Erro ao realizar reserva.';
      setErro(`❌ ${msg}`);
    } finally {
      setCarregando(false);
    }
  };

  return (
    <div className={temaEscuro ? 'dark' : ''}>
      <div className="min-h-screen font-sans transition-colors duration-300 bg-gray-50 text-gray-800 dark:bg-[#121212] dark:text-gray-100 p-4 md:p-8">
        <div className="max-w-4xl mx-auto">
          {/* Cabeçalho */}
          <div className="flex justify-between items-center mb-8">
            <h1 className="text-2xl md:text-3xl font-bold text-brand dark:text-violet-500 flex items-center gap-2">
              🍽️ Reserva Fácil
            </h1>
          </div>

          {/* Formulário */}
          <div className="bg-white dark:bg-[#1e1e1e] rounded-xl shadow-lg p-6 md:p-8 mb-8 border border-gray-100 dark:border-gray-800">
            <form onSubmit={buscarMesas} className="flex flex-col md:flex-row gap-4 items-end">
              <div className="w-full">
                <label className="block text-sm font-semibold mb-2 text-gray-600 dark:text-gray-300">Data</label>
                <input
                  type="date"
                  required
                  value={data}
                  onChange={e => setData(e.target.value)}
                  className="w-full p-3 rounded-lg border border-gray-300 bg-gray-50 focus:ring-2 focus:ring-brand focus:border-brand outline-none transition-all dark:bg-[#2c2c2c] dark:border-gray-700 dark:text-white"
                />
              </div>

              <div className="w-full">
                <label className="block text-sm font-semibold mb-2 text-gray-600 dark:text-gray-300">Horário</label>
                <input
                  type="time"
                  required
                  value={hora}
                  onChange={e => setHora(e.target.value)}
                  className="w-full p-3 rounded-lg border border-gray-300 bg-gray-50 focus:ring-2 focus:ring-brand focus:border-brand outline-none transition-all dark:bg-[#2c2c2c] dark:border-gray-700 dark:text-white"
                />
              </div>

              <div className="w-full md:w-32">
                <label className="block text-sm font-semibold mb-2 text-gray-600 dark:text-gray-300">Pessoas</label>
                <input
                  type="number"
                  min="1" max="20"
                  required
                  value={pessoas}
                  onChange={e => setPessoas(e.target.value)}
                  className="w-full p-3 rounded-lg border border-gray-300 bg-gray-50 focus:ring-2 focus:ring-brand focus:border-brand outline-none transition-all dark:bg-[#2c2c2c] dark:border-gray-700 dark:text-white"
                />
              </div>

              <button
                type="submit"
                disabled={carregando}
                className="w-full md:w-auto px-8 py-3 bg-brand hover:bg-brand-hover text-white font-bold rounded-lg shadow-md transition-transform active:scale-95 disabled:opacity-70 disabled:cursor-not-allowed"
              >
                {carregando ? '...' : '🔍 Buscar'}
              </button>
            </form>
          </div>

          {/* Mensagens */}
          {erro && (
            <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 mb-6 rounded shadow-sm dark:bg-red-900/30 dark:text-red-300">
              <p className="font-bold">Ops!</p>
              <p>{erro}</p>
            </div>
          )}

          {sucesso && (
            <div className="bg-green-100 border-l-4 border-green-500 text-green-700 p-4 mb-6 rounded shadow-sm dark:bg-green-900/30 dark:text-green-300">
              <p className="font-bold">Sucesso!</p>
              <p>{sucesso}</p>
            </div>
          )}

          {/* Resultados */}
          {buscaRealizada && (
            <div className="animate-fade-in">
              <h2 className="text-xl font-bold mb-6 border-b pb-2 border-gray-200 dark:border-gray-700">
                Mesas Disponíveis
              </h2>

              {mesasDisponiveis.length === 0 ? (
                <div className="bg-yellow-50 border border-yellow-200 text-yellow-800 rounded-lg p-6 text-center dark:bg-yellow-900/20 dark:border-yellow-700 dark:text-yellow-200">
                  🚫 Nenhuma mesa disponível para este horário.
                </div>
              ) : (
                <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
                  {mesasDisponiveis.map((mesa) => (
                    <div
                      key={mesa.id ?? mesa.mesaId}
                      className="group border-2 border-brand/30 hover:border-brand bg-white dark:bg-[#2a2a2a] rounded-xl p-4 text-center transition-all hover:shadow-lg hover:-translate-y-1"
                    >
                      <h3 className="text-2xl font-bold text-brand mb-1 group-hover:scale-110 transition-transform">
                        {mesa.numeroMesa ?? mesa.numero}
                      </h3>
                      <p className="text-sm text-gray-500 dark:text-gray-400 mb-4">
                        👤 {(mesa.capacidade ?? mesa.quantidadeLugares) || 0} lugares
                      </p>

                      <button
                        onClick={() => reservarMesa(mesa.id ?? mesa.mesaId)}
                        className="w-full py-2 bg-brand text-white text-sm font-bold rounded hover:bg-brand-hover transition-colors active:scale-95"
                      >
                        Reservar
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

        </div>
      </div>
    </div>
  );
}
