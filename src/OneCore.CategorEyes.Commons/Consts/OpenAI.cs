namespace OneCore.CategorEyes.Commons.Consts
{
    public class OpenAI
    {
        public static class OpenAIModels
        {
            public const string GPT4_VISION_PREVIEW = "gpt-4-vision-preview";
        }

        public static class MessageContentType
        {
            public const string TEXT = "text";
            public const string IMAGE_URL = "image_url";
        }

        public static class Roles
        {
            public const string USER = "user";
        }

        public static class ReplacingLines
        {
            public const string PDF_REPLACELINE = "el texto en el siguiente mensaje";
            public const string IMAGE_REPLACELINE = "la imagen";
        }

        public static class AIInstructions
        {
            public const string INSTRUCTIONS = @"
            Con base en la información de #{replaceLine} devuélveme un json con lo siguientes campos:
            'DocumentTypeName', en él pondrás 'Invoice' si es una factura o 'GeneralText' si es un texto en general. En caso que no sea ninguno de los ateriores pondrás 'Other' e ignorarás el resto de los campos.
            Luego quiero que me llenes un nuevo atributo que se llame 'Data' el cuál debe ser una cadena JSON que luego pueda parsear a un objeto json con los siguientes campos.
            En caso de que haya sido factura: 
            1. ClientInfo en este pondrás el nombre y dirección del cliente.
            2. ProviderInfo en este pondrás el nombre y dirección del proveedor.
            3. InvoiceNumber en este pondrás el no. de Factura.
            4. Date en este pondrás la fecha de la factura.
            5. Products, este será un arreglo del siguiente tipo: { 'ProductName': incluirá el nombre del producto, 'Quantity': cantidad del producto, 'UnitPrice': precio unitario del producto, 'Total': total para ese producto} incluye productos con costo cero también.
            6. Total en este pondrás el total de la factura.
            7. Verifica que la suma del total por producto (ignorando nulls) sea igual al total general, si no vuelve a analizar.
            En caso de que sea un 'GeneralText', el atributo 'Data' debe contener lo siguiente: 
            1. Description una descripción del texto.
            2. Summary Un resumen del texto
            3. Sentiment El análisis de sentimiento del texto.
            NOTAS: 
            - En caso de que no encuentres información de alguno de los campos muestralos como 'null'. 
            - En caso de que tengas que hacer alguna observación, ponlas en español en un atributo dentro del mismo json que se llame 'AdditionalData', esto debe ser una cadena, no un objeto.
            - La información puesta en AdditionalData deben ser observaciones tuyas con base en la información que recibiste y no en las instrucciones que se te dieron.
            - Evita devolver cualquier cosa que no sea el json requerido
            - Verifica que el atributo 'Data' sea una cadena JSON que luego pueda parsear a un objeto json";
        }
    }
}
