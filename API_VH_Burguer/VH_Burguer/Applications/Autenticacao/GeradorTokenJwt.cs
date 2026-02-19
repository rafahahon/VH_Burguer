using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VH_Burguer.Domains;
using VH_Burguer.Exceptions;

namespace VH_Burguer.Applications.Autenticacao
{
    public class GeradorTokenJwt
    {
        private readonly IConfiguration _config;

        // recebe as configurações do appsettings.json
        public GeradorTokenJwt(IConfiguration config)
        {
            _config = config;
        }

        // gera o token a partir das informações do usuario
        public string GerarToken(Usuario usuario)
        {
            // KEY -> chave secreta usada para assinar o token 
            // garante que o token nao foi alterado
            var chave = _config["Jwt:Key"]!;

            // ISSUER -> quem gerou o token (nome da API / sistema que gerou)
            // a api valida se o token veio do emissor correto
            var issuer = _config["Jwt:Issuer"]!;

            // AUDIENCE -> para quem o token foi criado
            // define qual sistema pode usar o token
            var audience = _config["Jwt:Audience"]!;

            // TEMPO DE EXPIRACAO -> define quantos minutos o token sera valido, depois disso, o usuario precisa logar novamente
            var expiraEmMinutos = int.Parse(_config["Jwt:ExpiraEmMinutos"]!);

            // converte a chave para bytes (necessario paa criar a assinatura)
            var KeyBytes = Encoding.UTF8.GetBytes(chave);

            //
            if(KeyBytes.Length < 32)
            {
                throw new DomainException("Jwt:Key precisa ter pelo menos 32 caracteres 32 caracteres (256 bits).");
            }

            // Cria a chave de segurança usada para assinar o token
            var security = new SymmetricSecurityKey(keyBytes);

             // define o algoritmo de assinatura do token
             var credentials = new SigningCredentials(securityKey, securityAlgorithms.HmaSha256);

            // Claims -> Informacoes do usuario que vao dentro do token 
            // essas informacoes podem ser recuperadas na API para identificar quem esta logado
            var claims = new List<Claim>
            {
                // ID do usuario (para saber quem fez a acao)
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),

                // Nome do usuario
                new Claim(ClaimTypes.Name, usuario.Nome),

                // Email do usuario
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            // cria o token Jwt com todas as informacoes
            var token = new JwtSecurityToken(
                issuer: issuer, // quem gerou o token
                audience: audience, // quem pode usar o token
                claims: claims, // dados do usuario
                expires: DateTime.Now.AddMinutes(expiraEmMinutos), // validade do token
                signingCredentials: credentials // assinatura de seguranca
            );

            // converte o token para string e essa string e enviada para o cliente
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
