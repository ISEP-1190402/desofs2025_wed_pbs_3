## **Methodology**

In this report, we will document the vulnerabilities found in pipeline scans, along with the help of Snky and Sonar.

We will analyze vulnerabilities in the code and dependencies and what alternatives exist to resolve them.

## ** Obejtivo**

It is to demonstrate the code that was poorly written, and come with the necessary corrections to avoid such types of vulnerabilities. In addition, we inform you of your CVES and their impacts.

## Code ##


![alt text](image-2.png)


Through SonarCloud, 18 vulnerability alerts were identified in the code. Below, we will describe and explain which part of the code they are in, what their risk is, and the CVE that is linked to this security flaw. However, we will not detail all of them, but some of the main ones, so that everyone can understand how this feature works.

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
Because strings can be easily extracted from an application’s source code or binary, credentials should not be hard-coded. This is particularly true for distributed or open-source applications.

In the past, this has led to the following vulnerabilities:

CVE-2019-13466

CVE-2018-15389

Credentials should be stored outside of the code in a configuration file, database, or secrets management service.

This rule flags instances of hard-coded credentials used in database and LDAP connections. It looks for hard-coded credentials in connection strings and for variable names that match any of the patterns in the provided list.


## **Negação de Serviço**

**Codigo com a falha de segurança:**

![alt text](<image-3.png>)

This code is processing user input using poorly designed regular expressions or without runtime validation. This could be an opening for DDOS.

**Onde esta o Risco?**

LibraryOnlineRentalSystem/Domain/Book/Author.cs

**Nivel do Risco**

- Medium

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

Failure to specify a timeout for regular expressions can lead to a denial of service attack. Set a timeout when System.Text.RegularExpressions processes untrusted input, because a malicious user could create a value that takes excessively long to evaluate.

- CWE - CWE-400 - Uncontrolled Resource Consumption
- CWE - CWE-1333 - Inefficient Regular Expression Complexity
regular-expressions.info - Uncontrolled Regular Expressions: Catastrophic Backtracking

## **Instalação automática dos pacotes**

**Codigo com a falha de segurança:**

![alt text](<image-5.png>)


**Onde esta o Risco?**

LibraryOnlineRentalSystem/Dockerfile


**Solução compatível**


![alt text](<image-4.png>)

**Nivel do Risco**

- Low

**Qual é o Risco?**

Automatic installation of recommended packages can lead to vulnerabilities in your Docker image.

Potentially unnecessary packages are installed via a known Debian package manager. These packages increase the attack surface of the created container, as they may contain unidentified vulnerabilities or malicious code. These packages can be used as part of a broader supply chain attack. In general, the more packages installed in a container, the weaker its security posture.
Depending on the vulnerabilities introduced, a malicious actor accessing such a container may be able to use them for privilege escalation.

Removing unused packages can also significantly reduce the size of your Docker image.

For added security, remove unused packages whenever possible and ensure that images undergo routine vulnerability scanning.

## **Criptografia de dados confidenciais**

**Codigo com a falha de segurança:**

![alt text](<image-6.png>)

**Onde esta o Risco?**

LibraryOnlineRentalSystem/Infrastructure/Services/DevelopmentEmailService.cs

**Nivel do Risco**

- Low

**Solução compatível**

Transit application data over a secure, authenticated, and encrypted protocol, such as TLS or SSH. 
Here are some alternatives to the most common plaintext protocols:

- Use ssh as an alternative to telnet.
- Use sftp, scp, or ftps instead of ftp.
- Use https instead of http.
- Use SMTP over SSL/TLS or SMTP with STARTTLS instead of plaintext SMTP.

Enable encryption of cloud component communications whenever possible.
Configure your application to block mixed content when rendering web pages.
If available, enforce operating system-level disabling of all cleartext traffic.

```
var urlHttps = "https://example.com";
var urlSftp = "sftp://anonymous@example.com";
var urlSsh = "ssh://anonymous@example.com";

usando var smtp = novo SmtpClient("host", 25) { EnableSsl = true };
usando var ssh = new MySsh.Client("host", porta);

```

**Qual é o Risco?**

Plain-text protocols such as FTP, Telnet, or HTTP do not encrypt the data being transported, nor do they have the ability to build an authenticated connection. This means that an attacker capable of sniffing network traffic could read, modify, or corrupt the transported content. These protocols are insecure because they expose applications to a wide range of risks:

Exposure of sensitive data
Traffic redirected to a malicious endpoint
Software update or installer infected with malware
Client-side code execution
Corruption of critical information
Even in the context of isolated networks, such as offline environments or segmented cloud environments, the insider threat exists. Therefore, attacks involving intercepted or tampered communications can still occur.

For example, attackers can successfully compromise previous layers of security by:

bypassing isolation mechanisms
compromising a network component
obtaining the credentials of an internal IAM account (either a service account or a real person)
In these cases, encrypting communications would decrease the chances of attackers exfiltrating data or stealing credentials from other network components. By implementing multiple layered security practices (segmentation and encryption, for example), the application will follow the principle of defense in depth .

Please note that the use of the http protocol is being deprecated by major web browsers.

In the past, this has led to the following vulnerabilities:

CVE-2019-6169
CVE-2019-12327
CVE-2019-11065

## Dependecias ##

**Dependencia com falha Log4net:**

![alt text](depen-1.png)

**Solução compatível**

Upgrade log4net to version 2.0.10 or higher.

**Onde esta o Risco?**

Log4net

**Nivel do Risco**

- Higher

**Qual é o Risco?**

XXE injection is a type of attack against an application that parses XML input. XML is a markup language that defines a set of rules for encoding documents in a format that is readable by both humans and machines. By default, many XML processors allow the specification of an external entity, a URI, that is dereferenced and evaluated during XML processing. When an XML document is being parsed, the parser can make a request and include the contents of the specified URI within the XML document.

Attacks can include the disclosure of local files, which may contain sensitive data such as passwords or private user data, using file schemes or relative paths in the system identifier.

For example, below is a sample XML document containing one XML element: username.
```
<xml>
<?xml version="1.0" encoding="ISO-8859-1"?>
   <username>John</username>
</xml>
```
An external XML entity - xxe, is defined using a system identifier and is present in a DOCTYPE header. These entities can access local or remote content. For example, the code below contains an external XML entity that would fetch the contents of /etc/passwd and display it to the user rendered by username.
```
<xml>
<?xml version="1.0" encoding="ISO-8859-1"?>
<!DOCTYPE foo [
   <!ENTITY xxe SYSTEM "file:///etc/passwd" >]>
   <username>&xxe;</username>
</xml>
```

CVE-2018-1285

**Dependencia com falha JWE:**

**Dependencia com a falha de segurança:**

![alt text](depe-2.png)
![alt text](depe-3.png)

**Onde esta o Risco?**

Upgrade Microsoft.IdentityModel.JsonWebTokens to version 5.7.0, 6.34.0, 7.1.2 or higher.

**Nivel do Risco**

- Medium

**Qual é o Risco?**

Affected versions of this package are vulnerable to Resource Exhaustion when processing JSON Web Encryption (JWE) tokens with high compression ratio. An attacker can cause excessive memory allocation and processing time during decompression, leading to a denial of service condition. This is only exploitable if the attacker has access to the public encryption key registered with the IDP (Entra ID).

CVE-2024-21319

Note: These were the vulnerabilities found in the backend, identified by sonar and snky. Given that it contains part of the source code and dependencies. In the pipelines we will also see that some were identified that belong to the frontend. However, we are not using the frontend in this project.

