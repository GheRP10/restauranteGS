# ReservaFácil GS

## 📌 Status do Projeto
**🟣 Em desenvolvimento**  
Funcionalidades centrais já estruturadas, com ajustes e melhorias contínuas sendo aplicadas no front-end, back-end e fluxo completo de reservas.

---

## Sobre o Projeto

Este projeto é uma solução **Full-Stack** desenvolvida para aperfeiçoar competências em:

- **arquitetura de software**
- **concorrência e consistência de dados**
- **design responsivo**
- **experiência do usuário (UX)**

O foco principal é um sistema de reservas de mesas com validação de disponibilidade em tempo real e prevenção de **overbooking**, garantindo que duas pessoas não consigam reservar a mesma mesa no mesmo horário.

---

## 🚀 Stack Tecnológica

### **Front-End**
- **React.js**
- **Vite**
- **Tailwind CSS**  
  - Suporte a **Dark Mode**
  - Layout responsivo pensado para desktop e mobile
- **Axios** para consumo da API

### **Back-End**
- **C#**
- **.NET 10**  
- **ASP.NET Core Web API**
- **Entity Framework Core** para acesso ao banco de dados
- **Fluent validation / regras de negócio manuais** para validar reservas

### **Banco de Dados**
- **PostgreSQL**
- Migrações e relacionamento entre:
  - Restaurante
  - Mesa
  - Reserva

### **Mensageria**
- **RabbitMQ**
- **RabbitMQ Web UI** para monitoramento das mensagens
- Publicação de notificações após confirmação de reserva

### **Infraestrutura & Deploy**
- **Railway**
  - Serviço do **Back-End (.NET)**  
  - Serviço do **Front-End (React/Vite)**  
  - Serviço de **Postgres**
  - Serviço de **RabbitMQ**
- **Docker** / Dockerfile para empacotar o back-end
- **Nginx** como servidor do front gerado pelo Vite

### **Arquitetura**
- API **RESTful**
- Uso de transações e checks de conflito para evitar overbooking:
  - Verificação de intervalos de horário (`DataHoraInicio` / `DataHoraFim`)
  - Status de reserva (ex.: ignorando reservas canceladas)
- Regras de negócio centralizadas no back-end para garantir integridade

---

## 💡 Destaque: Prevenção de Overbooking

O sistema trata **concorrência** de forma explícita.  
Ao receber uma nova tentativa de reserva, a API:

1. Converte a data/hora para UTC  
2. Calcula o intervalo da reserva com base no tempo padrão configurado para o restaurante  
3. Verifica se já existe alguma reserva ativa para a mesma mesa que:
   - não esteja cancelada  
   - e tenha interseção de horário com a nova reserva  
4. Só confirma a reserva se **não houver conflito**, garantindo:

- Reserva consistente  
- Integridade dos dados  
- Experiência confiável para o usuário  

Em seguida, a API ainda publica uma mensagem no **RabbitMQ**, permitindo que o sistema seja estendido com notificações, logs ou integrações futuras.

---

## 🖊️ Autora

**Gheizla Santos**  

Desenvolvido com:

- **C#**, **.NET 10**, **ASP.NET Core Web API**
- **React.js**, **Vite**, **Tailwind CSS**
- **PostgreSQL**
- **RabbitMQ**
- **Railway** (deploy dos serviços)
- **Docker** / **Nginx**

© 2025 - ReservaFácil GS