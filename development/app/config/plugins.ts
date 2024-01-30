export default ({ env }) => {
    const nodeEnv = env("NODE_ENV");
    if (nodeEnv === "production") {
        return {
            upload: {
                config: {
                    provider: "strapi-provider-upload-azure-storage",
                    providerOptions: {
                        account: env("STORAGE_ACCOUNT"),
                        accountKey: env("STORAGE_ACCOUNT_KEY"),
                        containerName: env("STORAGE_CONTAINER_NAME"),
                        defaultPath: "assets",
                    },
                },
            },
        };
    }
    else return {};
};
