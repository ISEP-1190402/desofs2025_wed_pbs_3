
***DAST- Teste Dinâmico de Segurança de Aplicações***

Este pipeline faz uma abordagem de segurança com o sistema em execução, ou seja, a aplicação está a correr, quando os testes são feitos por esta pipeline. seu objetivo é identificar vulnerabiliades de segurança enquanto o sistema está a ativo, com simulação de ataques reais de fora para dentro, sem acesso ao codigo-fonte, ou seja, são enviados varios tipos de endpoints, simulando diversos ataques, tais como: Sql Injection, XSS, CSPe entre ourtos.

Ferramentas comuns de DAST:

* OWASP ZAP

* Burp Suite (modo scanner)

* Acunetix

* Netsparker

Destas ferramentas acima citadas, iremos utilizar Owasp Zap.

# Estrutura DAST-ZAP

Aqui iremos descrever de forma simples, porem objetiva de como está criada a estrutrua do nosso DAST.

```
┌─────────────────────────────────────────────┐ 
│              Gatilhos (on)                  │
│  • Pull Request (PR)                        │
│  • workflow_dispatch (execução manual)      │
└─────────────────────┬───────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────┐ 
│             Job: dast-zap                   │
│       (executa somente em PR)               │
│                                             │
│ Ambiente: ubuntu-latest                     │
│ Timeout: 30 minutos                         │
└─────────────────────┬───────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────┐
│                  Steps                      │
├─────────────────────────────────────────────┤
│  1. Checkout Code                           │
│     └─ Baixa o código do repositório        │
│                                             │
│  2. Setup .NET SDK                          │
│     └─ Configura ambiente .NET 8.0          │
│                                             │
│  3. Build Docker Image                      │
│     ├─ Constrói imagem da API               │
│     └─ Tag: dast-api-test                   │
│                                             │
│  4. Create Docker Network                   │
│     └─ Rede: dast-network                   │
│                                             │
│  5. Run API Container                       │
│     ├─ Container: dast-api                  │
│     ├─ Porta: 8080:8080                     │
│     └─ Rede: dast-network                   │
│                                             │
│  6. Wait & Inspect API                      │
│     ├─ Aguarda 30 segundos                  │
│     ├─ Verifica status do container         │
│     └─ Exibe logs da API                    │
│                                             │
│  7. Test API Connectivity                   │
│     ├─ Testa endpoint /health               │
│     └─ Valida resposta da API               │
│                                             │
│  8. Run ZAP Security Scan                   │
│     ├─ Gera configuração do ZAP             │
│     ├─ Executa zap.sh baseline              │
│     ├─ Target: http://dast-api:8080         │
│     ├─ Timeout: 10 minutos                  │
│     └─ Fallback em caso de falha            │
│                                             │
│  9. Check & Process Reports                 │
│     ├─ Verifica arquivos gerados            │
│     ├─ Exibe resumo dos resultados          │
│     └─ Valida formato dos relatórios        │
│                                             │
│ 10. Upload Security Artifacts               │
│     ├─ HTML Report                          │
│     ├─ JSON Report                          │
│     ├─ XML Report                           │
│     └─ Container Logs                       │
│                                             │
│ 11. Error Handling                          │
│     ├─ Em caso de falha: exibe logs         │
│     ├─ Captura estado dos containers        │
│     └─ Debug de conectividade               │
│                                             │
│ 12. Cleanup Resources                       │
│     ├─ Para e remove containers             │
│     ├─ Remove rede Docker                   │
│     └─ Limpa recursos temporários           │
└─────────────────────────────────────────────┘

┌─────────────────────────────────────────────┐
│              Outputs/Artefatos              │
├─────────────────────────────────────────────┤
│ • ZAP HTML Report (relatório visual)        │
│ • ZAP JSON Report (dados estruturados)      │
│ • ZAP XML Report (formato padrão)           │
│ • Container Logs (debug/troubleshooting)    │
│ • Security Summary (resumo executivo)       │
└─────────────────────────────────────────────┘
```




