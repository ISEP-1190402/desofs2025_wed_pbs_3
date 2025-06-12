
### **Metodologia IAST- Teste Dinâmico de Segurança de Aplicações**



**Objetivos**


**Abordagem**

 

**Ferramentas comuns de IAST:**



Destas ferramentas acima citadas, iremos utilizar 

## **Estrutura DAST-ZAP**

Aqui iremos descrever de forma simples, porem objetiva de como está criada a estrutrua do nosso DAST.

```

```
**Secção Codigo-Fonte**

Apos apresentarmos a estrutura dos processos executados pelo IAST, iremos agora, revelar o codigo em si.

```


```

Abaixo descreveremos os resultados desta pipeline a corre, com as informações dos alertas e vulnerabilidades.

## **Reports** 



Abaixo disponibilizamos os resultados consolidados de cada relatório, contendo:

Total de alertas identificados: [5]

Nível de criticidade:

🟠 Alto: [1]

🟡 Médio: [2]

🔵 Baixo: [2]

Descrição do relatório:

✅ Descrição completa dos alertas

✅ Classificação de risco (CVSS)

✅ CVEs associados (com links para bases oficiais)

✅ Evidências técnicas (trechos de código/requisições)

✅ Recomendações de correção

 ![alt text](image.png)
 ![alt text](image-1.png)
 ![alt text](image-2.png)
 ![alt text](image-3.png)


 
 ## **Report Completo:**

 🔗 [Reports Passivo](./Deliverables/Phase%202%20-%20Sprint%202/Documentation/Pipeline/DAST/report_md.md)

 🔗 [Reports Profundidade](./Deliverables/Phase%202%20-%20Sprint%202/Documentation/Pipeline/DAST/report__baseline.md)

 

| Métrica          | Passivo | Ativo |
|:-----------------|--------:|:-----:|
| Total de Alertas |       5 |   7   |
| Crítico          |       0 |   0   |
| Alto (CVE-2024-XXXX) |       1 |   1   |
| Médio            |       2 |   2   |

## **Resumo da Análise DAST**  
- **Aplicação Testada:** API .NET 8 (Porta 5000)  
- **Vulnerabilidade Crítica:** 0  
- **Alerta Mais Grave:** [CVE-2024-47875] XSS em DOMPurify (CVSS 8.1)  
- **Recomendação Imediata:** Atualizar bibliotecas JavaScript  