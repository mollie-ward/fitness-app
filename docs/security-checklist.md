# Security Checklist - Fitness App Deployment

This document outlines security best practices and verification steps for deploying the Fitness App to Azure.

## ✅ Pre-Deployment Security Checklist

### Secrets Management

- [ ] **No secrets in source code**
  - Verify no connection strings in appsettings.json
  - Verify no API keys in source files
  - Verify no passwords in configuration files
  - Check .gitignore excludes .env files

- [ ] **GitHub Secrets configured**
  - All required secrets added to repository
  - Secrets use strong, randomly generated values
  - JWT secret is minimum 32 characters
  - SQL password meets complexity requirements

- [ ] **Azure Key Vault setup**
  - Key Vault created with soft delete enabled
  - RBAC authorization enabled
  - Managed identities configured for service access
  - Secrets stored in Key Vault, not environment variables (production)

### Infrastructure Security

- [ ] **Network Security**
  - SQL Database firewall configured (Azure services only)
  - Container Apps ingress properly configured
  - Static Web App allows only HTTPS traffic
  - No public endpoints except intended APIs

- [ ] **Identity and Access**
  - Managed identities used for service-to-service communication
  - Service Principal has least-privilege permissions
  - RBAC roles properly assigned
  - No admin users in production

- [ ] **Data Protection**
  - SQL Database encryption at rest enabled (default)
  - TLS/SSL enforced for all connections
  - Backup and retention policies configured
  - Soft delete enabled on critical resources

### Application Security

- [ ] **Backend API**
  - JWT authentication configured
  - CORS policy restricts allowed origins
  - Security headers implemented (X-Frame-Options, etc.)
  - Input validation on all endpoints
  - Rate limiting considered

- [ ] **Frontend**
  - Environment variables properly scoped (NEXT_PUBLIC_*)
  - No sensitive data in client-side code
  - API calls use HTTPS only
  - Content Security Policy configured

### CI/CD Pipeline Security

- [ ] **GitHub Actions**
  - Secrets not logged or exposed
  - Service Principal credentials secured
  - Container registry credentials not leaked
  - Workflow files reviewed for security issues

- [ ] **Container Security**
  - Base images from trusted sources (mcr.microsoft.com)
  - Multi-stage builds minimize attack surface
  - No secrets baked into images
  - Security scanning enabled (Trivy in PR validation)

## 🔐 Secret Rotation Schedule

| Secret | Rotation Period | Last Rotated | Next Due |
|--------|----------------|--------------|----------|
| Service Principal | 90 days | - | - |
| SQL Admin Password | 90 days | - | - |
| JWT Secret | 180 days | - | - |
| ACR Credentials | 90 days | - | - |
| Static Web App Token | 180 days | - | - |

## 🔍 Security Scanning

### Automated Scans

- **Trivy Security Scan**: Runs on every PR
  - Scans for vulnerabilities in dependencies
  - Scans Docker images for CVEs
  - Results uploaded to GitHub Security tab

- **Application Insights**: Monitors runtime security
  - Tracks failed authentication attempts
  - Monitors unusual access patterns
  - Alerts on exceptions and errors

### Manual Security Reviews

- **Code Review**: Before each deployment
  - Review for hardcoded secrets
  - Validate authentication/authorization
  - Check error handling doesn't leak sensitive info

- **Dependency Review**: Monthly
  - Check for known vulnerabilities in NuGet packages
  - Check for known vulnerabilities in npm packages
  - Update dependencies with security patches

## 🚨 Incident Response

### If Secrets are Compromised

1. **Immediate Actions**
   - Rotate compromised secret immediately
   - Revoke access tokens
   - Review access logs for unauthorized activity

2. **Investigation**
   - Identify scope of exposure
   - Review GitHub commit history
   - Check Azure Activity Log

3. **Remediation**
   - Update all affected secrets
   - Update GitHub Secrets
   - Redeploy affected services
   - Document incident and lessons learned

### If Service Principal is Compromised

1. Create new Service Principal
2. Update AZURE_CREDENTIALS secret in GitHub
3. Delete compromised Service Principal
4. Review all deployments made with old principal
5. Audit all Azure resources for unauthorized changes

## 📋 Compliance Considerations

### Data Privacy

- [ ] User data encrypted at rest and in transit
- [ ] Data retention policies configured
- [ ] PII handling procedures documented
- [ ] GDPR compliance reviewed (if applicable)

### Audit Logging

- [ ] Application Insights capturing all API calls
- [ ] Azure Activity Log enabled
- [ ] SQL Database auditing enabled
- [ ] Log retention meets compliance requirements

### Access Control

- [ ] Principle of least privilege enforced
- [ ] Multi-factor authentication required for Azure Portal
- [ ] Regular access reviews scheduled
- [ ] Separation of duties between dev and prod

## 🛡️ Security Best Practices

### Development Environment

1. **Never use production secrets in development**
   - Use separate Azure subscriptions or resource groups
   - Use different connection strings
   - Use test/dummy data only

2. **Local Development**
   - Never commit .env files
   - Use User Secrets for local development (.NET)
   - Use .env.local for frontend (auto-ignored)

3. **Code Reviews**
   - All changes require PR review
   - Security-focused review before merging
   - Automated security scanning on PRs

### Production Environment

1. **Monitoring**
   - Application Insights alerts configured
   - Failed authentication attempts monitored
   - Unusual traffic patterns detected

2. **Access Control**
   - Production access requires approval
   - Audit all production changes
   - Use managed identities instead of keys

3. **Disaster Recovery**
   - Automated backups configured
   - Recovery procedures documented
   - Regular DR drills performed

## 🔐 Encryption Standards

### Data at Rest

- **SQL Database**: Transparent Data Encryption (TDE) enabled by default
- **Key Vault**: Encrypted with Microsoft-managed keys
- **Application Insights**: Encrypted logs and telemetry

### Data in Transit

- **API Communication**: HTTPS/TLS 1.2+ required
- **Database Connections**: SSL enforced
- **Internal Services**: Encrypted connections between Azure services

## 📝 Security Documentation

### Required Documentation

- [ ] Security incident response plan
- [ ] Secret rotation procedures
- [ ] Access control policies
- [ ] Data classification guide
- [ ] Vulnerability disclosure policy

### Security Training

- [ ] Team trained on secure coding practices
- [ ] Team aware of OWASP Top 10
- [ ] Team knows how to report security issues
- [ ] Regular security awareness training

## ✅ Deployment Verification

### Post-Deployment Security Checks

1. **Verify HTTPS**
   ```bash
   curl -I https://<backend-url>/health
   # Should return 200 OK with HTTPS
   ```

2. **Test Authentication**
   ```bash
   curl https://<backend-url>/api/v1/protected
   # Should return 401 Unauthorized without token
   ```

3. **Check Security Headers**
   ```bash
   curl -I https://<backend-url>
   # Verify X-Frame-Options, X-Content-Type-Options present
   ```

4. **Validate CORS**
   ```bash
   curl -H "Origin: https://malicious-site.com" https://<backend-url>
   # Should reject unauthorized origins
   ```

5. **Test SQL Firewall**
   ```bash
   # Attempt connection from non-Azure IP
   # Should be rejected
   ```

### Health Check Verification

- [ ] Backend health endpoint returns 200
- [ ] Frontend health endpoint returns 200
- [ ] Database connection successful from backend
- [ ] Application Insights receiving telemetry

## 📞 Security Contacts

- **Security Lead**: [Contact Info]
- **Azure Admin**: [Contact Info]
- **DevOps Team**: [Contact Info]
- **Incident Response**: [Emergency Contact]

## 🔗 References

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Azure Security Best Practices](https://docs.microsoft.com/azure/security/fundamentals/best-practices-and-patterns)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)
- [GitHub Security Best Practices](https://docs.github.com/en/code-security)

---

**Last Updated**: [Date]  
**Next Review Due**: [Date + 90 days]  
**Approved By**: [Name]
