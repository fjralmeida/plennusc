<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="erro.aspx.cs" Inherits="appWhatsapp.ViewsApp.erro" %>
<!DOCTYPE html>
<html lang="pt-br">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Erro no Sistema | Plennus Connect</title>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
    <style>
        :root {
            --dark: #12181B;
            --dark-2: #0E1417;
            --green: #1E6B52;
            --green-light: #2ECC8F;
            --purple: #7B5FE0;
            --blue: #3E8DE0;
            --gray: #5B6670;
            --gray-light: #9AA5A0;
            --border: #E3E7E5;
        }

        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        html, body {
            height: 100%;
        }

        body {
            font-family: 'Poppins', sans-serif;
            background: #F4F6F5;
            color: var(--dark);
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 100vh;
            padding: 24px;
            position: relative;
            overflow-x: hidden;
        }

        /* Ambient bars, echoing the Plennus mark, anchored to the corner */
        .bars {
            position: fixed;
            top: -60px;
            right: -40px;
            display: flex;
            gap: 14px;
            opacity: 0.9;
            pointer-events: none;
        }

        .bars span {
            display: block;
            width: 22px;
            border-radius: 11px;
            animation: rise 2.4s cubic-bezier(.22,1,.36,1) both;
        }

        .bars span:nth-child(1) { height: 120px; background: var(--green-light); animation-delay: .05s; }
        .bars span:nth-child(2) { height: 170px; background: var(--purple); animation-delay: .15s; }
        .bars span:nth-child(3) { height: 210px; background: var(--blue); animation-delay: .25s; }

        @keyframes rise {
            from { transform: translateY(-40px); opacity: 0; }
            to { transform: translateY(0); opacity: 0.9; }
        }

        .wrap {
            display: flex;
            flex-direction: column;
            align-items: center;
            width: 100%;
            max-width: 460px;
            animation: fadeUp .6s cubic-bezier(.22,1,.36,1) both;
        }

        @keyframes fadeUp {
            from { opacity: 0; transform: translateY(14px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .brand {
            display: flex;
            align-items: center;
            gap: 10px;
            margin-bottom: 34px;
        }

        .brand .mark {
            display: flex;
            align-items: flex-end;
            gap: 3px;
            height: 26px;
        }

        .brand .mark i {
            display: block;
            width: 7px;
            border-radius: 3px;
        }

        .brand .mark i:nth-child(1) { height: 14px; background: var(--green-light); }
        .brand .mark i:nth-child(2) { height: 22px; background: var(--purple); }
        .brand .mark i:nth-child(3) { height: 26px; background: var(--blue); }

        .brand span {
            font-weight: 600;
            font-size: 18px;
            letter-spacing: 0.2px;
            color: var(--dark);
        }

        .brand span em {
            font-style: normal;
            color: var(--green);
        }

        .card {
            background: #FFFFFF;
            border: 1px solid var(--border);
            border-radius: 16px;
            box-shadow: 0 20px 45px -20px rgba(18, 24, 27, 0.18);
            padding: 40px 36px 34px;
            text-align: center;
            width: 100%;
        }

        .status-chip {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            background: #FDECEC;
            color: #C0392B;
            font-size: 12.5px;
            font-weight: 600;
            letter-spacing: 0.4px;
            padding: 6px 14px;
            border-radius: 999px;
            margin-bottom: 20px;
        }

        .status-chip .dot {
            width: 6px;
            height: 6px;
            border-radius: 50%;
            background: #E24C4C;
            animation: pulse 1.6s ease-in-out infinite;
        }

        @keyframes pulse {
            0%, 100% { opacity: 1; }
            50% { opacity: 0.35; }
        }

        .card h1 {
            font-weight: 600;
            font-size: 22px;
            color: var(--dark);
            margin-bottom: 12px;
        }

        .card p {
            color: var(--gray);
            font-size: 14.5px;
            font-weight: 300;
            line-height: 1.6;
            margin-bottom: 10px;
        }

        .card p:last-of-type {
            margin-bottom: 0;
        }

        .divider {
            height: 1px;
            background: var(--border);
            margin: 26px 0 24px;
        }

        .actions {
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            font-family: 'Poppins', sans-serif;
            font-size: 14.5px;
            font-weight: 500;
            text-decoration: none;
            border-radius: 10px;
            padding: 13px 20px;
            border: none;
            cursor: pointer;
            transition: transform .15s ease, background .15s ease, box-shadow .15s ease;
        }

        .btn-primary {
            background: var(--green);
            color: #ffffff;
            box-shadow: 0 10px 20px -10px rgba(30, 107, 82, 0.55);
        }

        .btn-primary:hover {
            background: #195A45;
            transform: translateY(-1px);
        }

        .btn-primary svg {
            transition: transform .15s ease;
        }

        .btn-primary:hover svg {
            transform: translateX(3px);
        }

        .support {
            margin-top: 22px;
            font-size: 12.5px;
            color: var(--gray-light);
        }

        .support a {
            color: var(--gray);
            font-weight: 500;
            text-decoration: none;
            border-bottom: 1px solid var(--border);
        }

        .support a:hover {
            color: var(--green);
            border-color: var(--green-light);
        }

        .footer-note {
            margin-top: 28px;
            font-size: 11.5px;
            color: var(--gray-light);
            letter-spacing: 0.3px;
        }

        @media (max-width: 480px) {
            .card {
                padding: 32px 24px 28px;
            }
            .bars {
                display: none;
            }
        }
    </style>
</head>
<body>

    <div class="bars" aria-hidden="true">
        <span></span><span></span><span></span>
    </div>

    <div class="wrap">

        <div class="brand">
            <span class="mark"><i></i><i></i><i></i></span>
            <span>Plennus <em>Connect</em></span>
        </div>

        <div class="card">
            <span class="status-chip"><span class="dot"></span>ERRO INESPERADO</span>

            <h1>Algo saiu do previsto</h1>
            <p>Encontramos um problema ao processar sua solicitação. Nossa equipe já foi notificada e está trabalhando para resolver o quanto antes.</p>
            <p>Volte ao início e tente novamente. Se o erro persistir, fale com o suporte.</p>

            <div class="divider"></div>

            <div class="actions">
                <a href="#" class="btn btn-primary" onclick="window.top.location.href='SignIn.aspx'; return false;">
                    Voltar ao login
                    <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M3.5 8H12.5" stroke="white" stroke-width="1.6" stroke-linecap="round"/>
                        <path d="M9 4.5L12.5 8L9 11.5" stroke="white" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/>
                    </svg>
                </a>
            </div>

            <div class="support">
                Precisa de ajuda? <a href="leonardo.ambrosio@vallorbeneficios.com.br">Fale com o suporte</a>
            </div>
        </div>

        <div class="footer-note">PLENNUS CONNECT &middot; ERP DE GESTÃO EMPRESARIAL</div>
    </div>

</body>
</html>
