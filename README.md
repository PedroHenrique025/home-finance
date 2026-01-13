# Projeto Controle de Gastos

## Tecnologias Utilizadas

### API
- ASP.NET Core  
- Entity Framework  
- SQLite (para banco de dados e persistência)

### Front-end
- React 18.2 (construção da interface)
- TypeScript 4.9 (utilizado principalmente para interação entre usuário e computador)
- React Router DOM 6 (gerenciamento de rotas)
- Axios (cliente HTTP para comunicação com a API)
- CSS3 (estilização customizada, sem uso de frameworks)

---

## Requisitos para Rodar a API

É necessário possuir o Entity Framework. Caso não tenha, utilize o terminal e, dentro do seguinte diretório:
`\home-finance.API\home-finance.API`
digite o seguinte comando: 

dotnet tool install --global dotnet-ef


Caso a pasta **Migrations** e o arquivo `home-finance.db` não estejam presentes no projeto, basta executar o arquivo `init.bat` para reiniciar as migrations e atualizar o banco de dados.  
Alternativamente, você pode executar os seguintes comandos pelo terminal, no mesmo diretório do `.bat`:

dotnet ef migrations add InitialCreate

dotnet ef database update


Além disso, é necessário ter instalado:
- O SDK mais atualizado do .NET (10.0)
- Uma IDE como Visual Studio Community ou Visual Studio Code

---

## Requisitos para Rodar o Front-end

É necessário ter o **Node.js** instalado para gerenciar os pacotes. Em seguida, execute os seguintes comandos no terminal:

npm install // Instala todas as dependências do projeto

npm start // Executa o projeto


---

## Lógica Adotada no Projeto

### API

A API foi desenvolvida com foco principal na lógica MVC e na segurança dos objetos, utilizando DTOs e encapsulamento.

Para garantir maior segurança e estabilidade dos dados, e seguindo boas práticas, todas as validações e cálculos relacionados aos dados do banco são realizados no back-end.

Para a persistência, optei pelo uso do SQLite, salvando todo o banco localmente em um arquivo. Configurei uma rotina com Migrations para que, sempre que o banco for apagado, ele possa ser recriado com alguns registros pré-cadastrados previamente programados.

Também adicionei a funcionalidade de edição de **Transações** e **Pessoas**, considerando a possibilidade de erros no momento do cadastro ou a necessidade de atualização de informações. Essa funcionalidade não impacta negativamente os demais fluxos do sistema.

Para **Categorias**, optei por não permitir edição. Embora isso reduza a liberdade do usuário, diminui significativamente a chance de erros e possíveis quebras na lógica do sistema.

Utilizei os padrões de nomenclatura **PascalCase**, **camelCase** e **_camelCase** em toda a API, seguindo o padrão oficial da Microsoft (Naming Conventions).

Também utilizei **XML Documentation Comments** para documentação do código, juntamente com **OpenAPI/Swagger Annotations** para documentar a API e seus endpoints, pois essas abordagens funcionam muito bem em conjunto e fazem parte do padrão oficial do ecossistema .NET.

---

### Front-end

O front-end foi desenvolvido com separação de responsabilidades (SoC), utilizando **components**, **services**, **types** e **config**, juntamente com um padrão de camadas bem definido.

A componentização é funcional, utilizando hooks modernos como `useState` e `useEffect`. Cada componente possui uma responsabilidade clara, mantendo apenas o estado local mínimo necessário.

O ciclo de vida é controlado e as validações ocorrem em cascata:
- No front-end, há validação imediata dos campos, com feedback visual e prevenção de requisições desnecessárias.
- No back-end, há validação autoritativa, regras de negócio bem definidas e proteção contra manipulação indevida dos dados.

O design adotado é simples, com uma interface limpa e moderna, sistema de cores enxuto e consistente, barra de navegação intuitiva, layout responsivo para diferentes tamanhos de tela e feedback visual claro sobre as ações do usuário. Algumas regras de negócio e informações importantes (como a maioridade da pessoa) também ficam explícitas na interface.

Além disso, utilizei componentes reutilizáveis, cards informativos e tabelas integradas a formulários consistentes.

---

## Documentação

Sobre a documentação, em resumo:

- **API**: grande parte da documentação está disponível no Swagger, complementada por este documento e por comentários pontuais no código.
- **Front-end (React)**: a documentação também é feita por este documento e de comentários no código, com o objetivo de esclarecer a lógica adotada.






