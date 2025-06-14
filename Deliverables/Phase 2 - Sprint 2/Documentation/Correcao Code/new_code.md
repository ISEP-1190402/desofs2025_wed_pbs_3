## ** Metodologia**
Neste report, iremos documentar vulnerabilidades encontradas com as varreduras feitas pelas pipelines, junto com o axuliio do Snky e Sonar. 

Iremos ver vulnerabilidades tanto em codigos quanto em dependencias e quais alternativas para resolução disto.

## ** Obejtivo**

É demonstar o codigo que foi mal feito, e vir com as correções necessárias para evitar tais tipos de vulnerabilidades. Alem de informamos as suas CVES e seus impactos.

## Code ##

![alt text](image-2.png)

Atraves do SonarCloud, foi identificado 18 alertas de vulnerabilidades no codigo, iremos abaixo descrever e explicar, em qual parte do codigo está, qual seu risco e sua CVE que esteja vinculada a esta falha de segurança.

**Onde esta o Risco?**

LibraryOnlineRentalSystem/appsettings.json


Codigo com a falha de segurança:

![alt text](<image-1.png>)

Solução compatível
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



## Dependecias ##