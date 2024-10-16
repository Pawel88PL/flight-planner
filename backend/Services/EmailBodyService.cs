using backend.Interfaces;

namespace backend.Services
{
    public class EmailBodyService : IEmailBodyService
    {
        public string ActivationEmailBody(string name, string link)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            background-color: #f4f4f4;
                            padding: 20px;
                        }}
                        .container {{
                            width: 80%;
                            margin: 0 auto;
                            padding: 20px;
                            border: 1px solid #ddd;
                            border-radius: 10px;
                            background-color: #fff;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        }}
                        .header {{
                            background-color: #4CAF50;
                            color: white;
                            padding: 10px 0;
                            text-align: center;
                            border-radius: 10px 10px 0 0;
                        }}
                        .content {{
                            padding: 20px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 0.9em;
                            color: #777;
                            text-align: center;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            margin-top: 20px;
                            font-size: 1em;
                            color: white;
                            background-color: #4CAF50;
                            text-decoration: none;
                            border-radius: 5px;
                        }}
                        .button:hover {{
                            background-color: #45a049;
                        }}
                        h2 {{
                            color: #4CAF50;
                        }}
                        p {{
                            margin: 10px 0;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Witaj {name}!</h1>
                        </div>
                        <div class='content'>
                            <p>Twoje konto zostało utworzone.</p>
                            <p>Aby aktywować swoje konto i móc korzystać z aplikacji, kliknij na poniższy link:</p>
                            <p><a href='{link}' class='button'>Aktywuj Konto</a></p>
                            <p>W razie problemów z aktywacją konta napisz do nas na adres <a href='mailto:sigid@sigid.pl'>sigid@sigid.pl</a></p>
                            <p>lub zadzwoń pod numer telefonu: +48 618 681 050</p>
                            <br>
                            <p>Pozdrawiamy,</p>
                            <p>Zakład Systemów Informatycznych SIGID Sp. z o. o.</p>
                        </div>
                        <div class='footer'>
                            <p>—</p>
                            <p>Ten e-mail został wygenerowany automatycznie, prosimy nie odpowiadać na tę wiadomość bezpośrednio.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        public string TwoFactorCodeEmailBody(string name, string twoFactorCode)
        {
            return $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        line-height: 1.6;
                        color: #333;
                        background-color: #f4f4f4;
                        padding: 20px;
                    }}
                    .container {{
                        width: 80%;
                        margin: 0 auto;
                        padding: 20px;
                        border: 1px solid #ddd;
                        border-radius: 10px;
                        background-color: #fff;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }}
                    .header {{
                        background-color: #4CAF50;
                        color: white;
                        padding: 10px 0;
                        text-align: center;
                        border-radius: 10px 10px 0 0;
                    }}
                    .content {{
                        padding: 20px;
                    }}
                    .footer {{
                        margin-top: 20px;
                        font-size: 0.9em;
                        color: #777;
                        text-align: center;
                    }}
                    .button {{
                        display: inline-block;
                        padding: 10px 20px;
                        margin-top: 20px;
                        font-size: 1em;
                        color: white;
                        background-color: #4CAF50;
                        text-decoration: none;
                        border-radius: 5px;
                    }}
                    .button:hover {{
                        background-color: #45a049;
                    }}
                    h2 {{
                        color: #4CAF50;
                    }}
                    p {{
                        margin: 10px 0;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Witaj {name}!</h1>
                    </div>
                    <div class='content'>
                        <p>Otrzymaliśmy prośbę o weryfikację dwuetapową dla Twojego konta w systemie ZION.</p>
                        <p>Twój kod do dwuetapowej weryfikacji to:</p>
                        <p><strong>{twoFactorCode}</strong></p>
                        <p>Wprowadź ten kod, aby zakończyć proces logowania.</p>
                        <br>
                        <p>Z wyrazami szacunku,</p>
                        <p>Zakład Systemów Informatycznych SIGID Sp. z o. o.</p>
                    </div>
                    <div class='footer'>
                        <p>—</p>
                        <p>Ten e-mail został wygenerowany automatycznie, prosimy nie odpowiadać na tę wiadomość bezpośrednio.</p>
                    </div>
                </div>
            </body>
            </html>";
        }
    }
}