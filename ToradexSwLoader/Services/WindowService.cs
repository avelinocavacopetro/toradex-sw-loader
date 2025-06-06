namespace ToradexSwLoader.Services
{
    using Microsoft.JSInterop;
    using System.Threading.Tasks;

    public class WindowService
    {
        private readonly IJSRuntime _js;

        public WindowService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<WindowDimension> GetDimensions()
        {
            return await _js.InvokeAsync<WindowDimension>("getDimensions");
        }

    }

    public class WindowDimension
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
