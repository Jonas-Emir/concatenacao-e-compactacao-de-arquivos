# Projeto de Gerenciamento de Arquivos

## Descrição
Este projeto é um sistema para o Gerenciamento de Arquivos desenvolvida em ASP.NET Core, projetada para oferecer uma plataforma para manipulação de arquivos com recursos como upload, visualização, download, manipulação para compactação e concatenação.

### Gerenciamento de Arquivos
- **Upload de Arquivos:** Permite aos usuários enviar novos arquivos para o sistema.
- **Listagem de Arquivos:** Apresenta uma visão geral de todos os arquivos armazenados, com opções para interagir individualmente com cada item.
- **Visualização de Arquivos:** Capacidade de visualizar o conteúdo de arquivos diretamente no navegador, melhorando a acessibilidade e conveniência.
- **Download de Arquivos:** Oferece a opção de baixar arquivos para o computador local do usuário, mantendo flexibilidade no acesso aos dados.
- **Compactação de Arquivos:** Funcionalidade para compactar arquivos selecionados em um arquivo ZIP.
- **Concatenação de Arquivos:** Permite combinar vários arquivos em um único arquivo.

## Demonstração
Veja abaixo uma demonstração visual do Sistema de Gerenciamento de Arquivos desenvolvido:

### Gif Demonstrativo
![Assista ao Gif de demonstração](https://github.com/Jonas-Emir/upload-e-download-de-arquivos/blob/feature/ImplementacaoUploadArquivos/demonstracaoUploadArquivos.gif)

### Captura de Tela
![Captura de Tela do Projeto](https://github.com/Jonas-Emir/upload-e-download-de-arquivos/blob/feature/ImplementacaoUploadArquivos/ImagemDemonstrativaColagem.png)

## Tecnologias Utilizadas
- **ASP.NET Core:** Framework robusto para desenvolvimento web, proporcionando alta performance e escalabilidade.
- **C#:** Linguagem utilizada para desenvolver a lógica do backend.
- **Entity Framework Core (EF Core):** ORM (Object-Relational Mapping) utilizado para acesso e manipulação de dados no banco de dados.
- **Bootstrap:** Para criação de interfaces responsivas e visualmente atrativas.

## Pré-requisitos e Configuração
- **Ambiente de Desenvolvimento:** Visual Studio.
- **SDK .NET Core:** Instalação necessária para compilar e executar o projeto.
- **Banco de Dados:** Configuração da string de conexão no arquivo `appsettings.json` e em ConfigureServices, suportando SQL Server ou outro banco compatível com EF Core.

    <div align="center">
  <h2>🚧 <strong>Em processo de refatoração</strong> 🚧</h2>
</div>
