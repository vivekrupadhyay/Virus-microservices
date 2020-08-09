using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Middleware
{
    public static class Extentions
    {
        public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoOptions>(configuration.GetSection("mongo"));
            services.AddSingleton(sp =>
            {
                var options = sp.GetService<IOptions<MongoOptions>>();

                return new MongoClient(options.Value.ConnectionString);
            });
            services.AddSingleton(sp =>
            {
                var options = sp.GetService<IOptions<MongoOptions>>();
                var client = sp.GetService<MongoClient>();

                return client.GetDatabase(options.Value.Database);
            });
        }

        public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new JwtOptions();
            var section = configuration.GetSection("jwt");
            section.Bind(options);
            services.Configure<JwtOptions>(section);
            services.AddSingleton<IJwtBuilder, JwtBuilder>();
            services.AddAuthentication()
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret))
                };
            });
        }
        public static string SwapitWith(this string KeyString, string Delimeter)
        {
            if (KeyString != null && KeyString != "")
            {
                Regex r = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                KeyString = r.Replace(KeyString, " ");
                string[] SplitArray = KeyString.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                KeyString = String.Join(Delimeter, SplitArray);
            }

            return KeyString;
        }
        public static string SwapItWithDash(string text)
        {
            string newText = "";

            try
            {
                newText = Regex.Replace(text, "[^a-zA-Z0-9]", " ");

                string[] SplitArray = newText.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                newText = String.Join("-", SplitArray);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return newText;
        }
        public static string SwapItWithUnderscore(string text)
        {
            string newText = "";

            try
            {
                newText = Regex.Replace(text, "[^a-zA-Z0-9]", "_");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return newText;
        }
        public static string GetCurrencyFormat(this string Price)
        {
            CultureInfo CInfo = new CultureInfo("hi-IN");
            if (Price != "")
            {
                Decimal Amount = Math.Round(Convert.ToDecimal(Price));
                return Amount.ToString("N", CInfo).Replace(".00", "");
            }
            else
            {
                string NullValue = "00";
                return NullValue;
            }

        }
        public static string GetAmountFormat(this string Price)
        {
            CultureInfo CInfo = new CultureInfo("hi-IN");
            if (Price != "")
            {
                Decimal Amount = Convert.ToDecimal(Price);
                return Amount.ToString("N", CInfo).Replace(".00", "");
            }
            else
            {
                return "00";
            }

        }
        public static string Sanatize(this string Text)
        {
            Text = Text.Replace("&lt;", "<");
            Text = Text.Replace("&gt;", ">");
            Text = Text.Replace("'", "''");
            Text = Text.Replace("[", "");
            Text = Text.Replace("]", "");
            Text = Text.Replace("(", "!@#$");
            Text = Text.Replace(")", "!@#$");
            string regEx = "<.*?>.*?</.*?>";
            string tagless = Regex.Replace(Text, regEx, string.Empty);
            tagless = Regex.Replace(tagless, @"\<[^\<\>]*\>", String.Empty);
            tagless = tagless.Replace("<", string.Empty).Replace(">", string.Empty);
            Text = tagless.Trim();
            //if (Text == "" || Text == null) throw new Exception("string can not be empty after applying ToClear() to it.");
            return Text;
        }
        public static string Capitalize(this string Value)
        {
            string strValue = Value;
            if (Value != "")
            {
                if (Value.Contains(" "))
                {
                    string[] array = Value.Split(' ');
                    strValue = "";
                    foreach (string item in array)
                    {
                        if (item != "")
                        {
                            string firchar = item.Substring(0, 1);
                            strValue += item.Replace(item, firchar.ToUpper() + item.Remove(0, 1)) + " ";
                        }
                    }
                }
                else
                {
                    string firstchar = strValue.Substring(0, 1);
                    strValue = firstchar.ToUpper() + strValue.Remove(0, 1);
                }
            }
            return strValue.Trim();
        }
    }
}
