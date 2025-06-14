## **Metodologia Pipeline**



**Objetivos**
Integrar práticas de segurança da informação ao longo do ciclo de vida de desenvolvimento de software (SDLC), por meio da automatização de testes e avaliações de segurança em pipelines CI/CD. 

Os objetivos específicos incluem:

- Detectar vulnerabilidades precocemente (shift-left security)

- Automatizar testes de segurança usando ferramentas como SAST, DAST, IAST e CSA

- Reduzir o tempo de correção de falhas encontradas

- Garantir conformidade com boas práticas da OWASP

- Melhorar a postura de segurança do software em ambientes de nuvem e locais




**Abordagem**

- Implementação de um pipeline CI/CD que execute testes de segurança a cada mudança no código.


## **Tabela Demonstrativa das Principais Pipelines**

| Característica                  | **SAST**                            | **DAST**                                | **IAST**                                      | **CSA**                                      |
| ------------------------------- | ----------------------------------- | --------------------------------------- | --------------------------------------------- | -------------------------------------------- |
| **Nome**                        | Static Application Security Testing | Dynamic Application Security Testing    | Interactive Application Security Testing      | Cloud Security Assessment                    |
| **Tipo de análise**             | Estática (código-fonte)             | Dinâmica (aplicação em execução)        | Híbrida (em tempo de execução com código)     | Estática/dinâmica de ambientes cloud         |
| **Momento de uso**              | Durante o desenvolvimento           | Após deploy (ambiente de teste)         | Durante testes manuais/automatizados          | Durante o design ou após deploy              |
| **Acesso ao código?**           | Sim                                 | Não                                     | Sim                                           | Às vezes                                     |
| **Cobertura**                   | Alta em código, baixa em execução   | Boa para vulnerabilidades reais         | Alta precisão                                 | Alta sobre infraestrutura                    |
| **Exemplos de ferramentas**     | SonarQube, Checkmarx, Fortify       | OWASP ZAP, Burp Suite, Acunetix         | Contrast Security, Seeker                     | Prisma Cloud, AWS Inspector, Wiz             |
| **Integração em pipeline**      | Fácil (antes de build)              | Moderada (requer app rodando)           | Moderada (requer instrumentação)              | Moderada (via APIs ou scans contínuos)       |
| **Vulnerabilidades detectadas** | Injeção, XSS, má prática de código  | Injeção, XSS, problemas de autenticação | Mescla vulnerabilidades estáticas e dinâmicas | Exposição de dados, má config. de rede, etc. |


