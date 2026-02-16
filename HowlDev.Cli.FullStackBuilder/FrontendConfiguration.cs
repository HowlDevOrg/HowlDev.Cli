namespace HowlDev.Cli.FullStackBuilder;

public enum FrontendPackageManager {
    Npm,
    Pnpm
}

public static class FrontendOptions {
    public static PackageDefinition[] Packages {
        get => [
            new PackageDefinition("Tailwind", "tailwindcss @tailwindcss/vite"),
            new PackageDefinition("Zod", "zod"),
            new PackageDefinition("React Router", "react-router"),
            new PackageDefinition("React Router DOM", "react-router-dom"),
            new PackageDefinition("React Bootstrap", "react-bootstrap bootstrap"),
            new PackageDefinition("Semantic UI", "semantic-ui-react semantic-ui-css"),
            new PackageDefinition("Headless UI", "@headlessui/react"),
            new PackageDefinition("Hot Toast", "react-hot-toast"),
        ];
    }
}
