# ZAP by Checkmarx Scanning Report

ZAP by [Checkmarx](https://checkmarx.com/).


## Summary of Alerts

| Risk Level | Number of Alerts |
| --- | --- |
| High | 1 |
| Medium | 2 |
| Low | 2 |
| Informational | 3 |




## Alerts

| Name | Risk Level | Number of Instances |
| --- | --- | --- |
| Vulnerable JS Library | High | 1 |
| Content Security Policy (CSP) Header Not Set | Medium | 2 |
| Missing Anti-clickjacking Header | Medium | 2 |
| Timestamp Disclosure - Unix | Low | 68 |
| X-Content-Type-Options Header Missing | Low | 7 |
| Information Disclosure - Suspicious Comments | Informational | 2 |
| Modern Web Application | Informational | 2 |
| User Agent Fuzzer | Informational | 10 |




## Alert Detail



### [ Vulnerable JS Library ](https://www.zaproxy.org/docs/alerts/10003/)



##### High (Medium)

### Description

The identified library appears to be vulnerable.

* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `var a="dompurify"+(n?"#"+n:"");try{return t.createPolicy(a,{createHTML:function(e){return e},createScriptURL:function(e){return e}})}catch(e){return console.warn("TrustedTypes policy "+a+" could not be created."),null}};function ne(){var t=arguments.length>0&&void 0!==arguments[0]?arguments[0]:te(),r=function(e){return ne(e)};if(r.version="2.3.10"`
  * Other Info: `The identified library DOMPurify, version 2.3.10 is vulnerable.
CVE-2024-47875
CVE-2025-26791
CVE-2024-48910
CVE-2024-45801
https://github.com/advisories/GHSA-gx9m-whjm-85jf
https://github.com/cure53/DOMPurify/releases/tag/3.2.4
https://github.com/cure53/DOMPurify/commit/d18ffcb554e0001748865da03ac75dd7829f0f02
https://github.com/cure53/DOMPurify/commit/6ea80cd8b47640c20f2f230c7920b1f4ce4fdf7a
https://github.com/advisories/GHSA-p3vf-v8qc-cwcr
https://github.com/cure53/DOMPurify/commit/0ef5e537a514f904b6aa1d7ad9e749e365d7185f
https://github.com/cure53/DOMPurify/security/advisories/GHSA-p3vf-v8qc-cwcr
https://nvd.nist.gov/vuln/detail/CVE-2024-45801
https://github.com/advisories/GHSA-vhxf-7vqr-mrjg
https://github.com/cure53/DOMPurify/security/advisories/GHSA-mmhx-hmjr-r674
https://github.com/cure53/DOMPurify/commit/d1dd0374caef2b4c56c3bd09fe1988c3479166dc
https://github.com/cure53/DOMPurify
https://github.com/advisories/GHSA-mmhx-hmjr-r674
https://github.com/cure53/DOMPurify/commit/26e1d69ca7f769f5c558619d644d90dd8bf26ebc
https://nvd.nist.gov/vuln/detail/CVE-2025-26791
https://nvd.nist.gov/vuln/detail/CVE-2024-47875
https://github.com/cure53/DOMPurify/security/advisories/GHSA-gx9m-whjm-85jf
https://github.com/cure53/DOMPurify/blob/0ef5e537a514f904b6aa1d7ad9e749e365d7185f/test/test-suite.js#L2098
https://ensy.zip/posts/dompurify-323-bypass
https://nsysean.github.io/posts/dompurify-323-bypass
https://github.com/cure53/DOMPurify/commit/1e520262bf4c66b5efda49e2316d6d1246ca7b21
https://nvd.nist.gov/vuln/detail/CVE-2024-48910
`

Instances: 1

### Solution

Upgrade to the latest version of the affected library.

### Reference


* [ https://owasp.org/Top10/A06_2021-Vulnerable_and_Outdated_Components/ ](https://owasp.org/Top10/A06_2021-Vulnerable_and_Outdated_Components/)


#### CWE Id: [ 1395 ](https://cwe.mitre.org/data/definitions/1395.html)


#### Source ID: 3

### [ Content Security Policy (CSP) Header Not Set ](https://www.zaproxy.org/docs/alerts/10038/)



##### Medium (High)

### Description

Content Security Policy (CSP) is an added layer of security that helps to detect and mitigate certain types of attacks, including Cross Site Scripting (XSS) and data injection attacks. These attacks are used for everything from data theft to site defacement or distribution of malware. CSP provides a set of standard HTTP headers that allow website owners to declare approved sources of content that browsers should be allowed to load on that page — covered types are JavaScript, CSS, HTML frames, fonts, images and embeddable objects such as Java applets, ActiveX, audio and video files.

* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: http://172.18.0.2:5000/index.html
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``

Instances: 2

### Solution

Ensure that your web server, application server, load balancer, etc. is configured to set the Content-Security-Policy header.

### Reference


* [ https://developer.mozilla.org/en-US/docs/Web/Security/CSP/Introducing_Content_Security_Policy ](https://developer.mozilla.org/en-US/docs/Web/Security/CSP/Introducing_Content_Security_Policy)
* [ https://cheatsheetseries.owasp.org/cheatsheets/Content_Security_Policy_Cheat_Sheet.html ](https://cheatsheetseries.owasp.org/cheatsheets/Content_Security_Policy_Cheat_Sheet.html)
* [ https://www.w3.org/TR/CSP/ ](https://www.w3.org/TR/CSP/)
* [ https://w3c.github.io/webappsec-csp/ ](https://w3c.github.io/webappsec-csp/)
* [ https://web.dev/articles/csp ](https://web.dev/articles/csp)
* [ https://caniuse.com/#feat=contentsecuritypolicy ](https://caniuse.com/#feat=contentsecuritypolicy)
* [ https://content-security-policy.com/ ](https://content-security-policy.com/)


#### CWE Id: [ 693 ](https://cwe.mitre.org/data/definitions/693.html)


#### WASC Id: 15

#### Source ID: 3

### [ Missing Anti-clickjacking Header ](https://www.zaproxy.org/docs/alerts/10020/)



##### Medium (Medium)

### Description

The response does not protect against 'ClickJacking' attacks. It should include either Content-Security-Policy with 'frame-ancestors' directive or X-Frame-Options.

* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: `x-frame-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: http://172.18.0.2:5000/index.html
  * Method: `GET`
  * Parameter: `x-frame-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``

Instances: 2

### Solution

Modern Web browsers support the Content-Security-Policy and X-Frame-Options HTTP headers. Ensure one of them is set on all web pages returned by your site/app.
If you expect the page to be framed only by pages on your server (e.g. it's part of a FRAMESET) then you'll want to use SAMEORIGIN, otherwise if you never expect the page to be framed, you should use DENY. Alternatively consider implementing Content Security Policy's "frame-ancestors" directive.

### Reference


* [ https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options ](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options)


#### CWE Id: [ 1021 ](https://cwe.mitre.org/data/definitions/1021.html)


#### WASC Id: 15

#### Source ID: 3

### [ Timestamp Disclosure - Unix ](https://www.zaproxy.org/docs/alerts/10096/)



##### Low (Low)

### Description

A timestamp was disclosed by the application/web server. - Unix

* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1467031594`
  * Other Info: `1467031594, which evaluates to: 2016-06-27 12:46:34.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1495990901`
  * Other Info: `1495990901, which evaluates to: 2017-05-28 17:01:41.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1501505948`
  * Other Info: `1501505948, which evaluates to: 2017-07-31 12:59:08.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1508970993`
  * Other Info: `1508970993, which evaluates to: 2017-10-25 22:36:33.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1518500249`
  * Other Info: `1518500249, which evaluates to: 2018-02-13 05:37:29.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1522805485`
  * Other Info: `1522805485, which evaluates to: 2018-04-04 01:31:25.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1537002063`
  * Other Info: `1537002063, which evaluates to: 2018-09-15 09:01:03.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1541459225`
  * Other Info: `1541459225, which evaluates to: 2018-11-05 23:07:05.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1546045734`
  * Other Info: `1546045734, which evaluates to: 2018-12-29 01:08:54.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1555081692`
  * Other Info: `1555081692, which evaluates to: 2019-04-12 15:08:12.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1575990012`
  * Other Info: `1575990012, which evaluates to: 2019-12-10 15:00:12.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1595750129`
  * Other Info: `1595750129, which evaluates to: 2020-07-26 07:55:29.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1607167915`
  * Other Info: `1607167915, which evaluates to: 2020-12-05 11:31:55.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1654270250`
  * Other Info: `1654270250, which evaluates to: 2022-06-03 15:30:50.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1694076839`
  * Other Info: `1694076839, which evaluates to: 2023-09-07 08:53:59.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1695183700`
  * Other Info: `1695183700, which evaluates to: 2023-09-20 04:21:40.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1731405415`
  * Other Info: `1731405415, which evaluates to: 2024-11-12 09:56:55.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1732584193`
  * Other Info: `1732584193, which evaluates to: 2024-11-26 01:23:13.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1747873779`
  * Other Info: `1747873779, which evaluates to: 2025-05-22 00:29:39.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1750603025`
  * Other Info: `1750603025, which evaluates to: 2025-06-22 14:37:05.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1779033703`
  * Other Info: `1779033703, which evaluates to: 2026-05-17 16:01:43.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1816402316`
  * Other Info: `1816402316, which evaluates to: 2027-07-24 04:11:56.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1856431235`
  * Other Info: `1856431235, which evaluates to: 2028-10-29 11:20:35.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1859775393`
  * Other Info: `1859775393, which evaluates to: 2028-12-07 04:16:33.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1894007588`
  * Other Info: `1894007588, which evaluates to: 2030-01-07 09:13:08.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1899447441`
  * Other Info: `1899447441, which evaluates to: 2030-03-11 08:17:21.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1914138554`
  * Other Info: `1914138554, which evaluates to: 2030-08-28 09:09:14.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1925078388`
  * Other Info: `1925078388, which evaluates to: 2031-01-01 23:59:48.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1955562222`
  * Other Info: `1955562222, which evaluates to: 2031-12-20 19:43:42.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1986661051`
  * Other Info: `1986661051, which evaluates to: 2032-12-14 18:17:31.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1996064986`
  * Other Info: `1996064986, which evaluates to: 2033-04-02 14:29:46.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `2003034995`
  * Other Info: `2003034995, which evaluates to: 2033-06-22 06:36:35.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `2007800933`
  * Other Info: `2007800933, which evaluates to: 2033-08-16 10:28:53.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `2024104815`
  * Other Info: `2024104815, which evaluates to: 2034-02-21 03:20:15.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1467031594`
  * Other Info: `1467031594, which evaluates to: 2016-06-27 12:46:34.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1495990901`
  * Other Info: `1495990901, which evaluates to: 2017-05-28 17:01:41.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1501505948`
  * Other Info: `1501505948, which evaluates to: 2017-07-31 12:59:08.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1508970993`
  * Other Info: `1508970993, which evaluates to: 2017-10-25 22:36:33.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1518500249`
  * Other Info: `1518500249, which evaluates to: 2018-02-13 05:37:29.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1522805485`
  * Other Info: `1522805485, which evaluates to: 2018-04-04 01:31:25.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1537002063`
  * Other Info: `1537002063, which evaluates to: 2018-09-15 09:01:03.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1541459225`
  * Other Info: `1541459225, which evaluates to: 2018-11-05 23:07:05.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1546045734`
  * Other Info: `1546045734, which evaluates to: 2018-12-29 01:08:54.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1555081692`
  * Other Info: `1555081692, which evaluates to: 2019-04-12 15:08:12.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1575990012`
  * Other Info: `1575990012, which evaluates to: 2019-12-10 15:00:12.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1595750129`
  * Other Info: `1595750129, which evaluates to: 2020-07-26 07:55:29.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1607167915`
  * Other Info: `1607167915, which evaluates to: 2020-12-05 11:31:55.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1654270250`
  * Other Info: `1654270250, which evaluates to: 2022-06-03 15:30:50.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1694076839`
  * Other Info: `1694076839, which evaluates to: 2023-09-07 08:53:59.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1695183700`
  * Other Info: `1695183700, which evaluates to: 2023-09-20 04:21:40.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1731405415`
  * Other Info: `1731405415, which evaluates to: 2024-11-12 09:56:55.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1732584193`
  * Other Info: `1732584193, which evaluates to: 2024-11-26 01:23:13.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1747873779`
  * Other Info: `1747873779, which evaluates to: 2025-05-22 00:29:39.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1750603025`
  * Other Info: `1750603025, which evaluates to: 2025-06-22 14:37:05.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1779033703`
  * Other Info: `1779033703, which evaluates to: 2026-05-17 16:01:43.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1816402316`
  * Other Info: `1816402316, which evaluates to: 2027-07-24 04:11:56.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1856431235`
  * Other Info: `1856431235, which evaluates to: 2028-10-29 11:20:35.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1859775393`
  * Other Info: `1859775393, which evaluates to: 2028-12-07 04:16:33.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1894007588`
  * Other Info: `1894007588, which evaluates to: 2030-01-07 09:13:08.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1899447441`
  * Other Info: `1899447441, which evaluates to: 2030-03-11 08:17:21.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1914138554`
  * Other Info: `1914138554, which evaluates to: 2030-08-28 09:09:14.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1925078388`
  * Other Info: `1925078388, which evaluates to: 2031-01-01 23:59:48.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1955562222`
  * Other Info: `1955562222, which evaluates to: 2031-12-20 19:43:42.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1986661051`
  * Other Info: `1986661051, which evaluates to: 2032-12-14 18:17:31.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1996064986`
  * Other Info: `1996064986, which evaluates to: 2033-04-02 14:29:46.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `2003034995`
  * Other Info: `2003034995, which evaluates to: 2033-06-22 06:36:35.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `2007800933`
  * Other Info: `2007800933, which evaluates to: 2033-08-16 10:28:53.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `2024104815`
  * Other Info: `2024104815, which evaluates to: 2034-02-21 03:20:15.`

Instances: 68

### Solution

Manually confirm that the timestamp data is not sensitive, and that the data cannot be aggregated to disclose exploitable patterns.

### Reference


* [ https://cwe.mitre.org/data/definitions/200.html ](https://cwe.mitre.org/data/definitions/200.html)


#### CWE Id: [ 497 ](https://cwe.mitre.org/data/definitions/497.html)


#### WASC Id: 13

#### Source ID: 3

### [ X-Content-Type-Options Header Missing ](https://www.zaproxy.org/docs/alerts/10021/)



##### Low (Medium)

### Description

The Anti-MIME-Sniffing header X-Content-Type-Options was not set to 'nosniff'. This allows older versions of Internet Explorer and Chrome to perform MIME-sniffing on the response body, potentially causing the response body to be interpreted and displayed as a content type other than the declared content type. Current (early 2014) and legacy versions of Firefox will use the declared content type (if one is set), rather than performing MIME-sniffing.

* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: `x-content-type-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: `This issue still applies to error type pages (401, 403, 500, etc.) as those pages are often still affected by injection issues, in which case there is still concern for browsers sniffing pages away from their actual content type.
At "High" threshold this scan rule will not alert on client or server error responses.`
* URL: http://172.18.0.2:5000/favicon-16x16.png
  * Method: `GET`
  * Parameter: `x-content-type-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: `This issue still applies to error type pages (401, 403, 500, etc.) as those pages are often still affected by injection issues, in which case there is still concern for browsers sniffing pages away from their actual content type.
At "High" threshold this scan rule will not alert on client or server error responses.`
* URL: http://172.18.0.2:5000/favicon-32x32.png
  * Method: `GET`
  * Parameter: `x-content-type-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: `This issue still applies to error type pages (401, 403, 500, etc.) as those pages are often still affected by injection issues, in which case there is still concern for browsers sniffing pages away from their actual content type.
At "High" threshold this scan rule will not alert on client or server error responses.`
* URL: http://172.18.0.2:5000/index.html
  * Method: `GET`
  * Parameter: `x-content-type-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: `This issue still applies to error type pages (401, 403, 500, etc.) as those pages are often still affected by injection issues, in which case there is still concern for browsers sniffing pages away from their actual content type.
At "High" threshold this scan rule will not alert on client or server error responses.`
* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: `x-content-type-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: `This issue still applies to error type pages (401, 403, 500, etc.) as those pages are often still affected by injection issues, in which case there is still concern for browsers sniffing pages away from their actual content type.
At "High" threshold this scan rule will not alert on client or server error responses.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: `x-content-type-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: `This issue still applies to error type pages (401, 403, 500, etc.) as those pages are often still affected by injection issues, in which case there is still concern for browsers sniffing pages away from their actual content type.
At "High" threshold this scan rule will not alert on client or server error responses.`
* URL: http://172.18.0.2:5000/swagger-ui.css
  * Method: `GET`
  * Parameter: `x-content-type-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: `This issue still applies to error type pages (401, 403, 500, etc.) as those pages are often still affected by injection issues, in which case there is still concern for browsers sniffing pages away from their actual content type.
At "High" threshold this scan rule will not alert on client or server error responses.`

Instances: 7

### Solution

Ensure that the application/web server sets the Content-Type header appropriately, and that it sets the X-Content-Type-Options header to 'nosniff' for all web pages.
If possible, ensure that the end user uses a standards-compliant and modern web browser that does not perform MIME-sniffing at all, or that can be directed by the web application/web server to not perform MIME-sniffing.

### Reference


* [ https://learn.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/compatibility/gg622941(v=vs.85) ](https://learn.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/compatibility/gg622941(v=vs.85))
* [ https://owasp.org/www-community/Security_Headers ](https://owasp.org/www-community/Security_Headers)


#### CWE Id: [ 693 ](https://cwe.mitre.org/data/definitions/693.html)


#### WASC Id: 15

#### Source ID: 3

### [ Information Disclosure - Suspicious Comments ](https://www.zaproxy.org/docs/alerts/10027/)



##### Informational (Low)

### Description

The response appears to contain suspicious comments which may help an attacker.

* URL: http://172.18.0.2:5000/swagger-ui-bundle.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `xxx`
  * Other Info: `The following pattern was used: \bXXX\b and was detected in likely comment: "//,""):-1!==l()(e).call(e,"#/components/schemas/")?e.replace(/^.*#\/components\/schemas\//,""):void 0)),i()(this,"getRefSchema",", see evidence field for the suspicious comment/snippet.`
* URL: http://172.18.0.2:5000/swagger-ui-standalone-preset.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `user`
  * Other Info: `The following pattern was used: \bUSER\b and was detected in likely comment: "//reactjs.org/docs/error-decoder.html?invariant="+t,r=1;r<arguments.length;r++)e+="&args[]="+encodeURIComponent(arguments[r]);re", see evidence field for the suspicious comment/snippet.`

Instances: 2

### Solution

Remove all comments that return information that may help an attacker and fix any underlying problems they refer to.

### Reference



#### CWE Id: [ 615 ](https://cwe.mitre.org/data/definitions/615.html)


#### WASC Id: 13

#### Source ID: 3

### [ Modern Web Application ](https://www.zaproxy.org/docs/alerts/10109/)



##### Informational (Medium)

### Description

The application appears to be a modern web application. If you need to explore it automatically then the Ajax Spider may well be more effective than the standard one.

* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `<script>
        if (window.navigator.userAgent.indexOf("Edge") > -1) {
            console.log("Removing native Edge fetch in favor of swagger-ui's polyfill")
            window.fetch = undefined;
        }
    </script>`
  * Other Info: `No links have been found while there are scripts, which is an indication that this is a modern web application.`
* URL: http://172.18.0.2:5000/index.html
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `<script>
        if (window.navigator.userAgent.indexOf("Edge") > -1) {
            console.log("Removing native Edge fetch in favor of swagger-ui's polyfill")
            window.fetch = undefined;
        }
    </script>`
  * Other Info: `No links have been found while there are scripts, which is an indication that this is a modern web application.`

Instances: 2

### Solution

This is an informational alert and so no changes are required.

### Reference




#### Source ID: 3

### [ User Agent Fuzzer ](https://www.zaproxy.org/docs/alerts/10104/)



##### Informational (Medium)

### Description

Check for differences in response based on fuzzed User Agent (eg. mobile sites, access as a Search Engine Crawler). Compares the response statuscode and the hashcode of the response body with the original response.

* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: `Header User-Agent`
  * Attack: `Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)`
  * Evidence: ``
  * Other Info: ``
* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: `Header User-Agent`
  * Attack: `Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)`
  * Evidence: ``
  * Other Info: ``
* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: `Header User-Agent`
  * Attack: `Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1)`
  * Evidence: ``
  * Other Info: ``
* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: `Header User-Agent`
  * Attack: `Mozilla/5.0 (Windows NT 10.0; Trident/7.0; rv:11.0) like Gecko`
  * Evidence: ``
  * Other Info: ``
* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: `Header User-Agent`
  * Attack: `Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3739.0 Safari/537.36 Edg/75.0.109.0`
  * Evidence: ``
  * Other Info: ``
* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: `Header User-Agent`
  * Attack: `Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)`
  * Evidence: ``
  * Other Info: ``
* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: `Header User-Agent`
  * Attack: `Mozilla/5.0 (compatible; Yahoo! Slurp; http://help.yahoo.com/help/us/ysearch/slurp)`
  * Evidence: ``
  * Other Info: ``
* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: `Header User-Agent`
  * Attack: `Mozilla/5.0 (iPhone; CPU iPhone OS 8_0_2 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Version/8.0 Mobile/12A366 Safari/600.1.4`
  * Evidence: ``
  * Other Info: ``
* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: `Header User-Agent`
  * Attack: `Mozilla/5.0 (iPhone; U; CPU iPhone OS 3_0 like Mac OS X; en-us) AppleWebKit/528.18 (KHTML, like Gecko) Version/4.0 Mobile/7A341 Safari/528.16`
  * Evidence: ``
  * Other Info: ``
* URL: http://172.18.0.2:5000
  * Method: `GET`
  * Parameter: `Header User-Agent`
  * Attack: `msnbot/1.1 (+http://search.msn.com/msnbot.htm)`
  * Evidence: ``
  * Other Info: ``

Instances: 10

### Solution



### Reference


* [ https://owasp.org/wstg ](https://owasp.org/wstg)



#### Source ID: 1


