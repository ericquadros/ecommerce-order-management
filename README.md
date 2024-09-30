# ecommerce-order-management
Repositório para o desafio final do treinamento C# do jeito certo apresentado pela Eximia na Bem 

## Qual a proposta?
O desafio consiste em desenvolver um sistema gerenciador de pedidos para e-commerce

## Como foi projetado
Foi projetado com a ideia de termos um projeto principal conectado ao banco de dados e que contem o domínio da aplicação.

- EcommerceOrderManagement: Projeto principal 
- EcommerceOrderManagement.WebApi: Projeto de Web Api para interações com os pedidos dos clientes.
- EcommerceOrderManagement.EventConsumer: Projeto consumidor de eventos Kafka para o processamento do pedido.
- EcommerceOrderManagement.SchedulerWorker: Projeto agendador para tarefas agendadas, como, envio de e-mail para o dono de cada produto.

C4 da aplicação
[Linkar imagem aqui]

Diagrama dos estados dos pedidos
[Linkar imagem aqui]

## Como rodar?

Entrar na pasta src   
``cd src``

Rodar no terminal:  
``docker compose up -d``  
``dotnet ef database update``