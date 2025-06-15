## ** Metodologia**
Neste report, iremos documentar vulnerabilidades encontradas com as varreduras feitas pelas pipelines, junto com o axuliio do Snky e Sonar. 

Iremos ver vulnerabilidades tanto em codigos quanto em dependencias e quais alternativas para resolução disto.

## ** Obejtivo**

É demonstar o codigo que foi mal feito, e vir com as correções necessárias para evitar tais tipos de vulnerabilidades. Alem de informamos as suas CVES e seus impactos.

## Code ##


![alt text](image-2.png)

Atraves do SonarCloud, foi identificado 18 alertas de vulnerabilidades no codigo, iremos abaixo descrever e explicar, em qual parte do codigo está, qual seu risco e sua CVE que esteja vinculada a esta falha de segurança. Entretanto não iremos detalhar todos mas alguns principais, para todos perceberem como é a funcionalidade deste recurso.

## **Detectcted Password**

**Onde esta o Risco?**

LibraryOnlineRentalSystem/appsettings.json


**Codigo com a falha de segurança:**

![alt text](<image-1.png>)

**Nivel do Risco**

- Alto

**Solução compatível**
```
string nome de usuário = "admin";
string senha = GetEncryptedPassword();
string usernamePassword = string.Format("usuário={0}&senha={1}", GetEncryptedUsername(), GetEncryptedPassword());
string url = $"scheme://{nomedeusuário}:{senha}@domínio.com";

string url2 = "http://guest:guest@domain.com"; // Compatível
const string Password_Property = "custom.password"; // Compatível
```
**Qual é o Risco?**

Como é fácil extrair strings do código-fonte ou binário de um aplicativo, as credenciais não devem ser codificadas. Isso é particularmente verdadeiro para aplicativos distribuídos ou de código aberto.

No passado, isso levou às seguintes vulnerabilidades:

CVE-2019-13466

CVE-2018-15389

As credenciais devem ser armazenadas fora do código em um arquivo de configuração, um banco de dados ou um serviço de gerenciamento de segredos.

Esta regra sinaliza instâncias de credenciais codificadas usadas em conexões de banco de dados e LDAP. Ela procura credenciais codificadas em strings de conexão e por nomes de variáveis ​​que correspondam a qualquer um dos padrões da lista fornecida.


## **Negação de Serviço**

**Codigo com a falha de segurança:**

![alt text](<image-3.png>)

Esse código  estiver processando entradas de usuários usando expressões regulares mal projetadas ou sem validação de tempo de execução. Pode ser uma abertura para um DDOS.

**Onde esta o Risco?**

LibraryOnlineRentalSystem/Domain/Book/Author.cs

**Nivel do Risco**

- Medio

**Solução compatível**

```
público void RegexPattern(string entrada)
{
    var emailPattern = novo Regex(".+@.+", RegexOptions.None, TimeSpan.FromMilliseconds(100));
    var isNumber = Regex.IsMatch(entrada, "[0-9]+", RegexOptions.None, TimeSpan.FromMilliseconds(100));
    var isLetterA = Regex.IsMatch(input, "(a+)+", RegexOptions.NonBacktracking); // .Net 7 e superior
    AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100)); // configuração de todo o processo
}
```

**Qual é o Risco?**

Não especificar um tempo limite para expressões regulares pode levar a um ataque de negação de serviço. Defina um tempo limite ao System.Text.RegularExpressionsprocessar entradas não confiáveis, pois um usuário mal-intencionado pode criar um valor cuja avaliação demore excessivamente.


- CWE - CWE-400 - Consumo descontrolado de recursos
- CWE - CWE-1333 - Complexidade de Expressão Regular Ineficiente
regular-expressions.info - Expressões regulares descontroladas: retrocesso catastrófico


## **Instalação automática dos pacotes**

**Codigo com a falha de segurança:**

![alt text](<image-5.png>)


**Onde esta o Risco?**

LibraryOnlineRentalSystem/Dockerfile


**Solução compatível**


![alt text](<image-4.png>)

**Nivel do Risco**

- Baixo

**Qual é o Risco?**

A instalação automática de pacotes recomendados pode levar a vulnerabilidades na imagem do Docker.

Pacotes potencialmente desnecessários são instalados por meio de um gerenciador de pacotes Debian conhecido. Esses pacotes aumentam a superfície de ataque do contêiner criado, pois podem conter vulnerabilidades não identificadas ou código malicioso. Esses pacotes podem ser usados ​​como parte de um ataque mais amplo à cadeia de suprimentos. Em geral, quanto mais pacotes são instalados em um contêiner, mais fraca é sua postura de segurança.
Dependendo das vulnerabilidades introduzidas, um agente malicioso que acesse tal contêiner pode usá-las para escalonamento de privilégios.
A remoção de pacotes não utilizados também pode reduzir significativamente o tamanho da sua imagem Docker.

Para maior segurança, remova pacotes não utilizados sempre que possível e garanta que as imagens sejam submetidas a verificações de vulnerabilidades de rotina.

## **Criptografia de dados confidenciais**

**Codigo com a falha de segurança:**

![alt text](<image-6.png>)

**Onde esta o Risco?**

LibraryOnlineRentalSystem/Infrastructure/Services/DevelopmentEmailService.cs

**Nivel do Risco**

- Baixo

**Solução compatível**

Transite os dados do aplicativo por meio de um protocolo seguro, autenticado e criptografado, como TLS ou SSH. 
Aqui estão algumas alternativas aos protocolos de texto simples mais comuns:

- Use ssh como alternativa a telnet.
- Use sftp, scp, ou ftpsem vez de ftp.
- Use https em vez de http.
- Use SMTPover SSL/TLSou SMTPwith STARTTLSem vez de SMTP de texto simples.

Habilite a criptografia das comunicações dos componentes da nuvem sempre que possível.
Configure seu aplicativo para bloquear conteúdo misto ao renderizar páginas da web.
Se disponível, aplique a desativação no nível do sistema operacional de todo o tráfego de texto não criptografado.

```
var urlHttps = "https://example.com";
var urlSftp = "sftp://anonymous@example.com";
var urlSsh = "ssh://anonymous@example.com";

usando var smtp = novo SmtpClient("host", 25) { EnableSsl = true };
usando var ssh = new MySsh.Client("host", porta);

```



**Qual é o Risco?**

Protocolos de texto simples, como ftp, telnet, ou httpnão criptografam os dados transportados, bem como não possuem a capacidade de construir uma conexão autenticada. Isso significa que um invasor capaz de rastrear o tráfego da rede pode ler, modificar ou corromper o conteúdo transportado. Esses protocolos não são seguros, pois expõem os aplicativos a uma ampla gama de riscos:

exposição de dados sensíveis
tráfego redirecionado para um endpoint malicioso
atualização ou instalador de software infectado por malware
execução de código do lado do cliente
corrupção de informações críticas
Mesmo no contexto de redes isoladas, como ambientes offline ou ambientes de nuvem segmentados, a ameaça interna existe. Portanto, ataques envolvendo comunicações interceptadas ou adulteradas ainda podem ocorrer.

Por exemplo, os invasores podem comprometer com sucesso camadas de segurança anteriores por meio de:

contornando mecanismos de isolamento
comprometendo um componente da rede
obter as credenciais de uma conta IAM interna (de uma conta de serviço ou de uma pessoa real)
Nesses casos, criptografar as comunicações diminuiria as chances de invasores vazarem dados ou roubarem credenciais de outros componentes da rede. Ao implementar diversas práticas de segurança em camadas (segmentação e criptografia, por exemplo), a aplicação seguirá o princípio da defesa em profundidade .

Observe que o uso do httpprotocolo está sendo descontinuado pelos principais navegadores da web .

No passado, isso levou às seguintes vulnerabilidades:

CVE-2019-6169
CVE-2019-12327
CVE-2019-11065


## Dependecias ##