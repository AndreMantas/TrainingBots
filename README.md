# TrainingBots

## Create a simple webchat page

Creata a new `index.html`file with this sample:

    <!DOCTYPE html>
    <html>
      <head>
        <link href="https://cdn.botframework.com/botframework-webchat/latest/botchat.css" rel="stylesheet" />
      </head>
      <body>
        <div id="bot"/>
        <script src="https://cdn.botframework.com/botframework-webchat/latest/botchat.js"></script>
        <script>
          BotChat.App({
            directLine: { secret: direct_line_secret },
            user: { id: 'userid' },
            bot: { id: 'botid' },
            resize: 'detect'
          }, document.getElementById("bot"));
        </script>
      </body>
    </html>

Note that you need to edit two settings:

- `direct_line_secret`: Secret from Direct Line channel
- `botid`: Is the `Bot handle` of your bot

Deploy your index.html file to `/site/wwwroot` using your favorite FTP client and the publish profile you got from Azure (check the FTP profile section and not the Web Deploy one).
For example, in FileZilla:

- Host: `publishUrl`
- Logon type: Normal
- User: `userName`
- Password: `userPWD`

Navigate to `/site/wwwroot` and copy the index.html file (delete the existing html file)
