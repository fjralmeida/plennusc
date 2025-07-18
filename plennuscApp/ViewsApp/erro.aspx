<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="erro.aspx.cs" Inherits="appWhatsapp.ViewsApp.erro" %>

<!DOCTYPE html>
<html lang="pt-br">
<head>
    <meta charset="utf-8" />
    <title>Erro no Sistema</title>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600&display=swap" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        * {
            font-family: 'Poppins', sans-serif;
        }

        body {
            background-color: #f8f9fa;
            margin: 0;
            padding: 0;
        }

        .container-erro {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            min-height: 100vh;
            padding: 20px;
        }

        .logo {
            max-width: 400px;
            margin-bottom: 20px;
        }

        .card-erro {
            background: #ffffff;
            border: none;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            padding: 30px;
            border-radius: 12px;
            text-align: center;
            max-width: 500px;
            width: 100%;
        }

        .card-erro h1 {
            color: #DC3545;
            font-weight: 600;
            margin-bottom: 15px;
        }

        .card-erro p {
            color: #6c757d;
            font-size: 15px;
        }

        .btn-voltar {
            margin-top: 20px;
            background-color: #4CB07A;
            color: white;
            border: none;
        }

        .btn-voltar:hover {
            background-color: #3da368;
        }
    </style>
</head>
<body>
    <div class="container-erro">
        <img  src="../Uploads/logo_plennus.png" alt="Logo Plennus Connect" class="logo" />

        <div class="card-erro">
            <h1>Ocorreu um erro</h1>
            <p>Algo deu errado em nossa aplicação. Estamos trabalhando para resolver o mais rápido possível.</p>
            <p>Por favor, tente novamente mais tarde ou entre em contato com o suporte.</p>
            <%--<a href="/ViewsApp/Signin.aspx" class="btn btn-voltar">Voltar ao Login</a>--%>
        </div>
    </div>
</body>
</html>